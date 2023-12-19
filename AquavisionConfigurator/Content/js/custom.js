var canvas = document.getElementById('can');
var c = canvas.getContext('2d');
canvas.width = window.innerWidth;
canvas.height = window.innerHeight;

var canvas_stack = new CanvasStack('can');
var layers1 = canvas_stack.createLayer();
var layers1_ctx = document.getElementById(layers1).getContext('2d');

var layers2 = canvas_stack.createLayer();
var layers2_ctx = document.getElementById(layers2).getContext('2d');

layers1_ctx.fillRect(0, 0, 100, 100);

layers2_ctx.fillStyle = 'red';
layers2_ctx.fillRect(100, 100, 100, 100);

layers1_ctx.clearRect(0, 0, window.innerWidth, window.innerHeight);