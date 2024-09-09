'use strict';

class PictureHelper {
    constructor(path, key) {
        this.Path = path;
        this.Key = key;
        this.loading = false;
        this.modalDOM = $("pictureModal");
    }

    static Load(path, key) {
        $.ajax({
            type: "post",
            url: "/Home/Thumbnail",
            async: true,
            data: {
                path: path,
                key: key
            }
        }).done(function (result) {
            var pictureElement = $('#img_' + key);
            pictureElement.attr("src", result);
            $('#pictureWrapper_' + key).find('span').hide();
            pictureElement.show();

            pictureElement.dblclick(function () { PictureHelper.LoadOriginal(path, key); });
            pictureElement.on('keydown', {
                key: 'space'
            }, event => {
                event.preventDefault();
                PictureHelper.LoadOriginal(path, key);
            });
        });
    }

    static LoadNext(path, key) {
        var me = this;
        if (!me.loading) {
            me.loading = true;
            $.ajax({
                type: "post",
                url: "/Home/GetNext",
                dataType: "json",
                async: true,
                data: { path: path, name: key }
            }).done(function (result) {
                me.LoadOriginal(result.path, result.key);
            });
        }
    }

    static LoadOriginal(path, key) {
            var me = this;
            if (!me.loading) {
                me.loading = true;
                me.modalDOM = $("#pictureModal");
                $.ajax({
                    type: "post",
                    url: "/Home/GetOriginal",
                    dataType: "json",
                    async: true,
                    data: { path: path, name: key }
                }).done(function (data) {
                    console.log("successfully loaded big image...");
                    window.CURRENTPICTURE = path;

                    $("#dynamic-content").css("background", "url(" + data.base64 + ")");
                    $("#dynamic-content").css("background-size", "contain");
                    $("#dynamic-content").css("background-position", "top center");
                    $("#dynamic-content").addClass("modal-body");

                    me.modalDOM.modal('show');

                    var zoomFactor = data.zoomFactor;
                    console.log("zoomFactor: " + zoomFactor);

                    $("#pictureModal").off('keydown', { keys: 'ctrl-alt-b' });
                    $("#pictureModal").on('keydown', {
                        keys: 'ctrl-alt-b'
                    }, event => {
                        event.preventDefault();
                        console.log("zoom in picture..." + key);
                        var backgroundSize = $("#dynamic-content").css("background-size");
                        var currentSize = parseInt(backgroundSize);
                        if (!currentSize || backgroundSize == "contain" || currentSize <= 500) {           
                            var zoom = 100;
                            if (currentSize) {
                                zoom = currentSize + zoomFactor;
                            }
                            $("#dynamic-content").css("background-size", zoom + "%");
                        }
                    });

                    $("#pictureModal").off('keydown', { keys: 'ctrl-alt-x' });
                    $("#pictureModal").on('keydown', {
                        keys: 'ctrl-alt-x'
                    }, event => {
                        event.preventDefault();
                        console.log("zoom out picture..." + key);
                        var backgroundSize = $("#dynamic-content").css("background-size");
                        var currentSize = parseInt(backgroundSize);
                        if (currentSize && currentSize >= (100 + zoomFactor)) {
                            var zoom = currentSize - zoomFactor;
                            $("#dynamic-content").css("background-size", zoom + "%");
                        } else if (currentSize && currentSize <= 100) {
                            $("#dynamic-content").css("background-size", "contain");
                            $("#dynamic-content").css("background-position", "top center");
                        }
                    });

                    $("#pictureModal").off('keydown', { keys: 'ctrl-alt-up' });
                    $("#pictureModal").on('keydown', {
                        keys: 'ctrl-alt-up'
                    }, event => {
                        event.preventDefault();
                        console.log("move picture down..." + key);
                        var backgroundPos = $("#dynamic-content").css('backgroundPosition').split(" ");
                        var xPos = parseInt(backgroundPos[0]);
                        var yPos = parseInt(backgroundPos[1]);
                        if (yPos >= 10) {
                            $("#dynamic-content").css("background-position", xPos + "% " + (yPos - 2) + "%");
                        }
                    });

                    $("#pictureModal").off('keydown', { keys: 'ctrl-alt-down' });
                    $("#pictureModal").on('keydown', {
                        keys: 'ctrl-alt-down'
                    }, event => {
                        event.preventDefault();
                        console.log("move picture up..." + key);
                        var backgroundPos = $("#dynamic-content").css('backgroundPosition').split(" ");
                        var xPos = parseInt(backgroundPos[0]);
                        var yPos = parseInt(backgroundPos[1]);
                        if (yPos < 100) {
                            $("#dynamic-content").css("background-position", xPos + "% " + (yPos + 2) + "%");
                        }
                    });

                    $("#pictureModal").off('keydown', { keys: 'ctrl-alt-ArrowLeft' });
                    $("#pictureModal").on('keydown', {
                        keys: 'ctrl-alt-ArrowLeft'
                    }, event => {
                        event.preventDefault();
                        console.log("move picture left..." + key);
                        var backgroundPos = $("#dynamic-content").css('backgroundPosition').split(" ");
                        var xPos = parseInt(backgroundPos[0]);
                        var yPos = parseInt(backgroundPos[1]);
                        if (xPos >= 10) {
                            $("#dynamic-content").css("background-position", (xPos - 2) + "% " + yPos + "%");
                        }
                    });

                    $("#pictureModal").off('keydown', { keys: 'ctrl-alt-ArrowRight' });
                    $("#pictureModal").on('keydown', {
                        keys: 'ctrl-alt-ArrowRight'
                    }, event => {
                        event.preventDefault();
                        console.log("move picture right..." + key);
                        var backgroundPos = $("#dynamic-content").css('backgroundPosition').split(" ");
                        var xPos = parseInt(backgroundPos[0]);
                        var yPos = parseInt(backgroundPos[1]);
                        if (xPos < 100) {
                            $("#dynamic-content").css("background-position", (xPos + 2) + "% " + yPos + "%");
                        }
                    });

                    $("#pictureModal").off('keydown', { keys: 'ctrl-alt-m' });
                    $("#pictureModal").on('keydown', {
                        keys: 'ctrl-alt-m'
                    }, event => {
                        event.preventDefault();
                        console.log("move picture..." + key);
                        $.ajax({
                            method: "POST",
                            url: "/Home/MovePicture",
                            dataType: "json",
                            data: { path: path, newPath: window.CURRENTALBUM }
                        }).done(function (result) {
                            var result = JSON.parse(result);
                            console.log("moved image: " + result);
                        }).fail(function () {
                            console.log("Error occured...");
                        });
                    });

                    me.loading = false;
                }).fail(function () {
                    console.log("Error occured...");
                });
            } else {
                console.log("still loading another picture...");
            }
    }

    static CreateAlbum(albumname, directory) {
        $.ajax({
            method: "POST",
            url: "/Home/NewAlbum",
            dataType: "json",
            data: { name: albumname }
        }).done(function (album) {
            if (album) {
                console.log("New album-path: " + album.path);
                window.CURRENTALBUM = album.path;
            }
        }).fail(function () {
            console.log("Error occured...");
        });
    }
}

class PictureViewer {
    constructor(){}

    static LoadViewer(){
        if (window.module) module = window.module;

        $(document).on('keydown', {
            keys: 'F1'
        }, event => {
            event.preventDefault();
            console.log("open help...");
            var dialog = $('<div id="dialog" title="Help">'
                + 'ctrl + i = dev tools <br/>'
                + 'ctrl + alt + n = create new album <br/>'
                + 'ctrl + o = open a folder <br/>'
                + 'ctrl + left = previous picture <br/>'
                + 'ctrl + right = next picture <br/>'
                + 'ctrl + alt + m = move picture to current album<br/>'
                + 'ctrl + alt + left = go to left side <br/>'
                + 'ctrl + alt + right = go to right side <br/>'
                + 'ctrl + alt + top = go to top side <br/>'
                + 'ctrl + alt + bottom = go to bottom side <br/>'
                + 'ctrl + alt + b = zoom in <br/>'
                + 'ctrl + alt + x = zoom out <br/>'
                + '</div > ');
            var body = $('body');
            body.append(dialog);
            dialog.dialog();
        });

        $(document).on('keydown', {
            keys: 'ctrl-i'
        }, event => {
            event.preventDefault();
            console.log("open dev tools...");
            ElectronWindow.prototype.OpenDevTools();
        });

        window.CURRENTALBUM = "";
        window.CURRENTPICTURE = "";

        $(document).on('keydown', {
            keys: 'ctrl-alt-n'
        }, event => {
            event.preventDefault();
            console.log("create new album...");
            document.querySelector('#albumDialog').showModal();
        });

        $(document).on('keydown', {
            keys: 'ctrl-o'
        }, event => {
            event.preventDefault();
            console.log("open folder...");
            $("#thumbnails").load("/Home/OpenFolder");

            $.ajax({
                type: "post",
                url: "/Home/GetWindowSize",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                async: true
            }).done(function (size) {
                console.log("window-Size: " + size);
                $("#thumbnails").css("width", size[0]).css("height", size[1]);
            });
        });

        $("#pictureModal").on('keydown', {
            keys: 'ctrl-ArrowRight'
        }, event => {
            event.preventDefault();
            console.log("next picture...");
            if (window.CURRENTFOLDER && window.CURRENTPICTURE) {
                $.ajax({
                    type: "post",
                    url: "/Home/GetNext",
                    async: true,
                    data: {
                        currentfolder: window.CURRENTFOLDER,
                        currentpicture: window.CURRENTPICTURE
                    }
                }).done(function (path) {
                    PictureHelper.LoadOriginal(path);
                });
            }
        });

        $("#pictureModal").on('keydown', {
            keys: 'ctrl-ArrowLeft'
        }, event => {
            event.preventDefault();
            console.log("previous picture...");
            if (window.CURRENTFOLDER && window.CURRENTPICTURE) {
                $.ajax({
                    type: "post",
                    url: "/Home/GetPrevious",
                    async: true,
                    data: {
                        currentfolder: window.CURRENTFOLDER,
                        currentpicture: window.CURRENTPICTURE
                    }
                }).done(function (path) {
                    PictureHelper.LoadOriginal(path);
                });
            }
        });

        //enable focus for Divs to allow key-combinations
        var divs = document.getElementsByTagName('div');
        for (var i = 0, len = divs.length; i < len; i++) {
            divs[i].setAttribute('tabindex', '0');
        }

        $(window).on('resize', function () {
            var win = $(this);
            console.log("window-Size: " + win.width() + " x " + win.height());
            $("#thumbnails").css("width", win.width()).css("height", win.height());
        });
    }
}