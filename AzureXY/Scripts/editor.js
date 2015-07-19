
var CANVAS_WIDTH = 720;
var CANVAS_HEIGHT = 640;

function Editor(element) {
    this.canvas = $(element);
    this.target = this.canvas.attr('data-target');
    this.lines = [];
    this.currentLine = 0;
    this.penMode = "DRAW";
    this.locateButtons();
    this.registerButtons(); 
};

Editor.prototype = {
    "locateButtons": function () {
        this.buttonDraw = $('.toolbox-button-draw[data-target="' + this.target + '"]').addClass("btn-primary");
        this.buttonErase = $('.toolbox-button-erase[data-target="' + this.target + '"]');
        this.buttonNewline = $('.toolbox-button-newline[data-target="' + this.target + '"]');
        this.buttonWrite = $('.toolbox-button-newline[data-target="' + this.target + '"]');
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

        }
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
    "addPoint": function(point) {
        console.log("adding", point.x, point.y);
    },
    "actionNewline": function (event) {
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

    },
};

$(function () {
    $("canvas.toolbox-canvas").each(function (i, element) {
        var e = new Editor(element);
    });
})