function initEditor(id, value) {
    monaco.editor.create(document.getElementById(id), {
        value: value,
        language: 'javascript'
    });
}