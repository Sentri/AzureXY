
var CANVAS_WIDTH = 720;
var CANVAS_HEIGHT = 640;
var CANVAS_ERASE_TOLERANCE = 10;

function lineLenght(point1, point2) {
    var xs = Math.pow(point2.x - point1.x, 2);
    var ys = Math.pow(point2.y - point1.y, 2); 
    return Math.sqrt(xs + ys);
}

function parsePoint(line) {
    var px = parseInt(line.substring(1, 4), 10);
    var py = parseInt(line.substring(4, 7), 10);
    return { "x": CANVAS_WIDTH - py, "y": px };
}

function helperReadLines(text) {
    // emulate xytable and return a list of lines
    var mode = 0;
    var penDown = false;
    var point = null;
    var textLines = text.replace("\r", "").split("\n");
    var instructions = [];
    var current = null;

    for (var k in textLines) {
        var line = textLines[k];
        var start = line.substring(0, 1);
        if (mode == 0) {
            if (line == "XYB10") {
                mode = 1;
            }
        } else {
            switch (start) {
                case "U":
                    penDown = false;
                    instructions.push(current);
                    current = null;
                    break;
                case "D":
                    penDown = true;
                    current = [point];
                    break;
                case "M":
                    point = parsePoint(line);
                    if (penDown) {
                        current.push(point);
                    }
                    break;
                case "E":
                    mode = 0;
                    break;
                default:
                    break;
            }
        }
    }
    return instructions;
}

function Editor(element) {
    this.canvas = $(element);
    this.target = this.canvas.attr('data-target');
    this.lines = [];
    this.currentLine = 0;
    this.penMode = "DRAW";
    this.image = null;
    this.locateButtons();
    this.registerButtons();
    this.setupSearch();
    this.loadFromText();
    this.requestUpdate();
}

Editor.prototype = {
    "locateButtons": function () {
        this.buttonDraw = $('.toolbox-button-draw[data-target="' + this.target + '"]').addClass("btn-primary");
        this.buttonErase = $('.toolbox-button-erase[data-target="' + this.target + '"]');
        this.buttonNewline = $('.toolbox-button-newline[data-target="' + this.target + '"]');
        this.buttonWrite = $('.toolbox-button-write[data-target="' + this.target + '"]');
        this.buttonImage = $('.toolbox-button-image[data-target="' + this.target + '"]');
    },
    "registerButtons": function () {
        this.buttonDraw.on("touchstart mousedown", this.actionDraw.bind(this));
        this.buttonErase.on("touchstart mousedown", this.actionErase.bind(this));
        this.buttonNewline.on("touchstart mousedown", this.actionNewline.bind(this));
        this.buttonWrite.on("touchstart mousedown", this.actionWrite.bind(this));
        this.canvas.on("touchend mouseup", this.actionTap.bind(this));
        this.canvas.on("touchstart", function (e) {
            this.lastStartEvent = e;
            this.lastMoveEvent = null;
        }.bind(this));
        this.canvas.on("touchmove", function (e) {
            this.lastMoveEvent = e;
        }.bind(this));
        this.buttonImage.magnificPopup({
            type:'inline',
            midClick: true
        });
    },
    "updateCanvas": function () {
        var ctx = this.canvas[0].getContext("2d");
        ctx.rect(0, 0, CANVAS_WIDTH, CANVAS_HEIGHT);
        ctx.fillStyle = "white";
        ctx.fill();
        if (this.image != null) {
            var iw = this.image.width;
            var ih = this.image.height;
            var ratio = iw / ih;
            var scaler;
            if (ratio > (CANVAS_WIDTH / CANVAS_HEIGHT)) {
                scaler = CANVAS_WIDTH / iw;
            } else {
                scaler = CANVAS_HEIGHT / ih;
            }
            ctx.drawImage(this.image, 0, 0, iw * scaler, ih * scaler);
            ctx.rect(0, 0, CANVAS_WIDTH, CANVAS_HEIGHT);
            ctx.fillStyle = "rgba(255, 255, 255, 0.8)";
            ctx.fill();
        }
        for (var k in this.lines) {
            ctx.beginPath();
            for (var i = 0; i < this.lines[k].length; i++) {
                var point = this.lines[k][i];
                if (i == 0) {
                    ctx.moveTo(point.x, point.y);
                } else {
                    ctx.lineTo(point.x, point.y);
                }
            }
            ctx.strokeStyle = "black";
            ctx.stroke();
        }
    },
    "requestUpdate": function () {
        window.requestAnimationFrame(this.updateCanvas.bind(this));
    },
    "actionTap": function(event) {
        if (this.penMode === "DRAW") {
            if (this.currentLine >= this.lines.length) {
                // append a new line
                this.lines.push([]);
            }
            var point = this.getPointScaled(event);
            this.addPoint(point);
        } else {
            var point = this.getPointScaled(event);
            this.removeLine(point);
        }
    },
    "scalePoint": function(point) {
        var scaler = CANVAS_WIDTH / this.canvas.width();
        return {"x": point.x / scaler, "y": point.y / scaler};
    },
    "flipPoint": function (point) {
        // flips point counterclockwise 90 degrees
        var xb = point.y;
        var yb = CANVAS_WIDTH - point.x;
        return { "x": xb, "y": yb };
    },
    "padPoint": function (point) {
        var xi, yi, xs, ys;
        xi = Math.floor(point.x);
        yi = Math.floor(point.y);
        if (xi < 10) {
            xs = "00" + xi;
        } else if (xi < 100) {
            xs = "0" + xi;
        } else {
            xs = "" + xi;
        }
        if (yi < 10) {
            ys = "00" + yi;
        } else if (yi < 100) {
            ys = "0" + yi;
        } else {
            ys = "" + yi;
        }
        return xs+ys;
    },
    "getPointScaled": function(event) {
        if (event.type == "touchend") {
            if (this.lastMoveEvent == null) {
                var position = this.canvas.offset();
                var touchX = this.lastStartEvent.originalEvent.targetTouches[0].pageX - position.left;
                var touchY = this.lastStartEvent.originalEvent.targetTouches[0].pageY - position.top;
                var scaler = CANVAS_WIDTH / this.canvas.width();
                event.preventDefault();
                return { "x": touchX * scaler, "y": touchY * scaler };
            }
        } else if (event.type == "mouseup") {
            var scaler = CANVAS_WIDTH / this.canvas.width();
            return { "x": event.offsetX * scaler, "y": event.offsetY * scaler };
        }
    },
    "removeLine": function (touchPoint) {
        if (touchPoint) {
            for (var k in this.lines) {
                for (var i = 0; i < this.lines[k].length; i++) {
                    var point = this.lines[k][i];
                    if (lineLenght(point, touchPoint) < CANVAS_ERASE_TOLERANCE) {
                        this.lines.splice(k, 1);
                        this.actionNewline();
                        this.requestUpdate();
                        return;
                    }
                }
            }
        }
    },
    "addPoint": function (point) {
        if (point) {
            this.lines[this.currentLine].push(point);
            this.requestUpdate();
        }
    },
    "actionNewline": function () {
        this.currentLine = this.lines.length;
    },
    "actionDraw": function (event) {
        this.buttonDraw.removeClass("btn-default").addClass("btn-primary");
        this.buttonErase.removeClass("btn-primary").addClass("btn-default");
        this.penMode = "DRAW";
    },
    "actionErase": function (event) {
        this.buttonErase.removeClass("btn-default").addClass("btn-primary");
        this.buttonDraw.removeClass("btn-primary").addClass("btn-default");
        this.penMode = "ERASE";
    },
    "actionWrite": function (event) {
        var target = $("#" + this.target);
        console.log(target);
        var instructions = "XYB10\r\n";
        for (var k in this.lines) {
            for (var i in this.lines[k]) {
                var point = this.flipPoint(this.lines[k][i]);
                instructions += "M"+this.padPoint(point)+"\r\n";
                if (i == 0) {
                    instructions += "D\r\n";
                }
            }
            instructions += "U\r\n";
        }
        instructions += "M010010\r\n";
        instructions += "E\r\n";
        target.val(instructions);
    },
    "setImage": function (location) {
        if (location == "") {
            this.image = null;
        } else {
            var img = new Image();
            img.src = location;
            this.image = img;
            img.addEventListener("load", this.requestUpdate.bind(this), false);
        }
    },
    "loadFromText": function (text) {
        var text = $("#" + this.target).val();
        if (text && text.length > 0) {
            this.lines = helperReadLines(text);
        };
    },
    "setupSearch": function () {
        var searchButton = $("#bing-popup button.toolbox-search-button");
        searchButton.click(this.searchBing.bind(this));
    },
    "searchBing": function () {
        var searchQuery = $("#bing-popup input.toolbox-search").val();
        $.getJSON("/Tables/SearchImages?query=" + encodeURIComponent(searchQuery))
        .done(this.searchCallback.bind(this))
        .fail(function (a, b, c) {
            console.log("error", a, b, c);
        });
    },
    "searchCallback": function (data, status, o) {
        var searchTarget = $("#bing-popup div.toolbox-popup-target").empty();
        for (var i in data) {
            var img = data[i];
            var div = $("<div>").addClass("col-md-2").append(
                $("<img>").attr("src", img.ThumbnailURL)
                    .attr("data-uri", img.ImageURL)
                    .click(this.clickImage.bind(this))
            );
            searchTarget.append(div);
        }
    },
    "clickImage": function(e) {
        var imageURL = e.target.getAttribute("data-uri");
        this.setImage(imageURL);
        $.magnificPopup.close();
    },
};

function SVGCreator(element) {
    this.instructions = $(element).html();
    this.element = $(element).empty();
    this.lines = helperReadLines(this.instructions);
    this.svg = null;
    this.createSVG();
}

SVGCreator.prototype = {
    "createSVG": function () {
        var svg = document.createElementNS("http://www.w3.org/2000/svg", "svg");
        svg.setAttribute("width", ""+CANVAS_WIDTH);
        svg.setAttribute("height", ""+CANVAS_HEIGHT);
        svg.setAttribute("viewBox", "0 0 "+CANVAS_WIDTH+" "+CANVAS_HEIGHT);

        for (var k in this.lines) {
            var polyline = document.createElementNS("http://www.w3.org/2000/svg", "polyline");
            polyline.setAttribute("fill", "none");
            polyline.setAttribute("stroke", "black");
            var points = "";
            for (var i in this.lines[k]) {
                var point = this.lines[k][i];
                points += point.x + "," + point.y + " ";
            }
            polyline.setAttribute("points", points);
            svg.appendChild(polyline);
        }

        this.element.append(svg);
    },
};

function SignalR() {
    this.con = $.hubConnection();
    this.hub = this.con.createHubProxy("moveShape");
    console.log("hub initialized");

    this.hub.on("shapeMoved", this.shapeMoved.bind(this));
    this.hub.on("startDrawing", this.startDrawing.bind(this));
    this.hub.on("stopDrawing", this.stopDrawing.bind(this));

}

SignalR.prototype = {
    "shapeMoved": function (x, y) {
        console.log("shape moved", x, y);
    },
    "startDrawing": function () {
        console.log("start drawing");
    },
    "stopDrawing": function () {
        console.log("stop drawing");
    }
}

$(function () {
    $("canvas.toolbox-canvas").each(function (i, element) {
        var e = new Editor(element);
    });
    $("div.toolbox-thumbnail").each(function (i, element) {
        var e = new SVGCreator(element);
    });
    var sr = new SignalR();
})