class AjaxHelper {
    constructor() { }
    static RenderBody(url, callback) {
        $("#content").load(url, callback);
    }
}