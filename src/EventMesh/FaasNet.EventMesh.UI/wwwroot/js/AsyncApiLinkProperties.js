function initEditor(id, value) {
    return monaco.editor.create(document.getElementById(id), {
        value: value,
        language: 'javascript',
        automaticLayout: true
    });
}