
var CANVAS_WIDTH = 720;
var CANVAS_HEIGHT = 640;
var CANVAS_ERASE_TOLERANCE = 10;

function lineLenght(point1, point2) {
    var xs = Math.pow(point2.x - point1.x, 2);
    var ys = Math.pow(point2.y - point1.y, 2); 
    return Math.sqrt(xs + ys);
}

function Editor(element) {
    this.canvas = $(element);
    this.target = this.canvas.attr('data-target');
    this.lines = [];
    this.currentLine = 0;
    this.penMode = "DRAW";
    this.locateButtons();
    this.registerButtons();
    this.requestUpdate();
};

Editor.prototype = {
    "locateButtons": function () {
        this.buttonDraw = $('.toolbox-button-draw[data-target="' + this.target + '"]').addClass("btn-primary");
        this.buttonErase = $('.toolbox-button-erase[data-target="' + this.target + '"]');
        this.buttonNewline = $('.toolbox-button-newline[data-target="' + this.target + '"]');
        this.buttonWrite = $('.toolbox-button-write[data-target="' + this.target + '"]');
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
    },
    "updateCanvas": function () {
        console.log(this);
        var ctx = this.canvas[0].getContext("2d");
        ctx.rect(0, 0, CANVAS_WIDTH, CANVAS_HEIGHT);
        ctx.fillStyle = "white";
        ctx.fill();
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
        var yb = CANVAS_HEIGHT - point.x;
        return { "x": xb, "y": xb };
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
        instructions += "E\r\n";
        target.val(instructions);
    },
};

$(function () {
    $("canvas.toolbox-canvas").each(function (i, element) {
        var e = new Editor(element);
    });
})