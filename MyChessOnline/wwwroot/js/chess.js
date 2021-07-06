var divSquare = '<div id="s$coord" class = "square $color"></div>'
var divFigure = '<div id="f$coord" class = "figure">$figure</div>'
var map;
var isDragging = false
var mat = false;
var pat = false;
var your_move = document.getElementById("IsYourMove").innerHTML;
var fen = document.getElementById("GameFen").innerHTML;
var game_id = document.getElementById("GameId").innerHTML;
var chess = new Chess(fen);

$(function () {
    start();
})

hubConnection.on('Chess', function (FEN,move, user) {
    //почему-то тут не работает Chess = new Chess();
    your_move = your_move == 1 ? 0 : 1;
    document.getElementById("IsYourMove").textContent = your_move;
    document.getElementById("GameFen").textContent = FEN;
    chess.load(FEN);
    mat = chess.in_checkmate();
    pat = chess.in_stalemate();
    if ((mat || pat) == true) {
        let side = chess.turn();
        hubConnection.invoke('OverGame', mat, game_id, side);
    }
        
    let new_fen = parser_fen(FEN);
    showFigures(new_fen);
});



function start() {
    chess = new Chess(fen);
    map = new Array(64)
    addSquares()
    showFigures(parser_fen(fen));
    setDroppable()
}

function setDraggable() {
    $('.figure').draggable({
        start: function (event, ui) {
            isDragging = true;
        }
    });
}

function setDroppable() {
    $('.square').droppable({
        drop: function (event, ui) {
            //console.log(event);
            //console.log(ui);
            var frCoord = ui.draggable.attr('id').substring(1);
            var toCoord = this.id.substring(1);
            var fr = position_figure(frCoord)
            var to = position_figure(toCoord)

            if (your_move == 1) {
                var flag = chess.move('' + fr + to, { sloppy: true })
            }
            else
                var flag = null;

            if (flag != null) {
                moveFigure(frCoord, toCoord);
                let fen = chess.fen().toString();
                document.getElementById("IsYourMove").textContent = 0;
                hubConnection.invoke('Move', fen, fr + to, game_id);
            }
            else {
                console.log(map[frCoord] + 'какая фигура')
                $('#s' + frCoord).html(divFigure
                    .replace('$coord', frCoord)
                    .replace('$figure', getChessSymbol(map[frCoord])));
                setDraggable();
            }

            isDragging = false;
        }
    });
}

function moveFigure(frCoord, toCoord) {
    console.log('move from ' + frCoord + ' to ' + toCoord);
    figure = map[frCoord];
    showFigureAt(frCoord, '1');
    showFigureAt(toCoord, figure);
}

function addSquares() {
    $('.board').html('');
    for (var i = 0; i < 64; i++) {
        $('.board').append(divSquare
            .replace('$coord', i)
            .replace('$color',
                isBlackSquareAt(i) ? 'black' : 'white'));
    }
    console.log('addchess');
}

function showFigures(figures) {
    for (var coord = 0; coord < 64; coord++)
        showFigureAt(coord, figures.charAt(coord))
}

function showFigureAt(coord, figure) {
    if (map[coord] == figure) return;
    map[coord] = figure
    $('#s' + coord).html(divFigure
        .replace('$coord', coord)
        .replace('$figure', getChessSymbol(figure)));
    setDraggable();
}

function getChessSymbol(figure) {
    switch (figure) {
        case 'K': return '&#9812;';
        case 'Q': return '&#9813;';
        case 'R': return '&#9814;';
        case 'B': return '&#9815;';
        case 'N': return '&#9816;';
        case 'P': return '&#9817;';
        case 'k': return '&#9818;';
        case 'q': return '&#9819;';
        case 'r': return '&#9820;';
        case 'b': return '&#9821;';
        case 'n': return '&#9822;';
        case 'p': return '&#9823;';
        default: return '';
    }
}

function isBlackSquareAt(coord) {
    if (coord % 2) {
        if (Math.floor(coord / 8) % 2 == 0)//если находимся на чётной вершине
            return true
        else//если находим не на четной вершине
            return false
    }
    else {
        if (Math.floor(coord / 8) % 2 == 0)
            return false
        else
            return true
    }
}

function position_figure(coord) {
    position_y = 8 - Math.floor(coord / 8)
    position_x = Math.floor(coord % 8)
    switch (position_x) {
        case 0: return 'a' + position_y;
        case 1: return 'b' + position_y;
        case 2: return 'c' + position_y;
        case 3: return 'd' + position_y;
        case 4: return 'e' + position_y;
        case 5: return 'f' + position_y;
        case 6: return 'g' + position_y;
        case 7: return 'h' + position_y;
        default: return '';
    }
    return position
}



function parser_fen(fen) {
    let new_fen = fen.split(' ')
    new_fen = new_fen[0];
    while (new_fen.includes("/")) {
        new_fen = new_fen.replace("/", "");
    }
    while (new_fen.includes("8")) {
        new_fen = new_fen.replace("8", "11111111");
    }
    while (new_fen.includes("7")) {
        new_fen = new_fen.replace("7", "1111111");
    }
    while (new_fen.includes("6")) {
        new_fen = new_fen.replace("6", "111111");
    }
    while (new_fen.includes("5")) {
        new_fen = new_fen.replace("5", "11111");
    }
    while (new_fen.includes("4")) {
        new_fen = new_fen.replace("4", "1111");
    }
    while (new_fen.includes("3")) {
        new_fen = new_fen.replace("3", "111");
    }
    while (new_fen.includes("2")) {
        new_fen = new_fen.replace("2", "11");
    }
    return new_fen;
}