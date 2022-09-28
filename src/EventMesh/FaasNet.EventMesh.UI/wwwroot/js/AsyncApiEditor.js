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