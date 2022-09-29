function getSize(id) {
    const elt = document.getElementById(id);
    return {
        Width: elt.offsetWidth,
        Height: elt.offsetHeight
    };
}

function getApplicationMousePosition(id, clientX, clientY) {
    const elt = document.getElementById(id);
    const ctm = elt.getScreenCTM();
    return {
        X: (clientX - ctm.e) / ctm.a,
        Y: (clientY - ctm.f) / ctm.d
    };
}

function computeCoordinate(id, clientX, clientY) {
    const elt = document.getElementById(id);
    const ctm = elt.getScreenCTM();
    const point = elt.createSVGPoint();
    point.clientX = clientX;
    point.clientY = clientY;
    const result = point.matrixTransform(ctm.inverse());
    return {
        X: result.clientX,
        Y : result.clientY
    };
}