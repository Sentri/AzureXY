
function Editor(element) {
    this.canvas = $(element);
    this.target = this.canvas.attr("data-target");
    this.lines = [];
    this.locateButtons();
    this.registerButtons();
    this.registerTouch();    
};

Editor.prototype = {
    "locateButtons": function () {
        
    },
    "registerButtons": function () {

    },
    "registerTouch": function () {

    },
    "actionNewline": function () {

    },
    "actionDraw": function () {

    },
    "actionErase": function () {

    },
};

$(function () {
    $("canvas.toolbox-canvas").each(function (i, element) {
        var e = new Editor(element);
    });
})