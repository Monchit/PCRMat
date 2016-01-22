/**
 * jQueryUI Tabbed Dialog
 * Based on http://forum.jquery.com/topic/combining-ui-dialog-and-tabs
 * Modified to work by Joseph T. Parsons
 * For jQueryUI 1.10 and jQuery 2.0
 */
$.fn.tabbedDialog = function (dialogOptions, tabOptions) {
    this.tabs(tabOptions);
    this.dialog(dialogOptions);



    // Create the Tabbed Dialogue
    var tabul = this.find('ul:first');
    this.parent().addClass('ui-tabs').prepend(tabul).draggable('option', 'handle', tabul);
    tabul.append($('.ui-dialog-titlebar-close').css({ 'top': '1.5em', 'right': '1em' }));
    this.prev().remove();
    tabul.addClass('ui-dialog-titlebar-tabbed');

    this.attr("tabIndex", -1).attr("role", "dialog");



    // Make Only The Content of the Tab Tabbable
    this.bind("keydown.ui-dialog", function (event) {
        if (event.keyCode !== $.ui.keyCode.TAB) {
            return;
        }


        var tabbables = $(":tabbable", this).add("ul.ui-tabs-nav.ui-dialog-titlebar-tabbed > li > a"),
          first = tabbables.filter(":first"),
          last = tabbables.filter(":last");


        if (event.target === last[0] && !event.shiftKey) {
            first.focus(1);
            return false;
        }
        else if (event.target === first[0] && event.shiftKey) {
            last.focus(1);
            return false;
        }
    });



    // Give the First Element in the Dialog Focus
    var hasFocus = this.find(":tabbable");
    if (!hasFocus.length) {
        hasFocus = uiDialog.find(".ui-dialog-buttonpane :tabbable");
        if (!hasFocus.length) {
            hasFocus = uiDialog;
        }
    }
    hasFocus.eq(0).focus();
}