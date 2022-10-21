function initEditor(id, value, readonly) {
    return monaco.editor.create(document.getElementById(id), {
        value: value,
        language: 'javascript',
        automaticLayout: true,
        readOnly: readonly
    });
}