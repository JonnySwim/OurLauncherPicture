"use strict";
var ElectronWindow = class { constructor() { } };

ElectronWindow.prototype.CloseWindow = function () {
    $(document).ready(function () {
        $.ajax({
            url: "/home/CloseWindow"
                }).done(function () {
                console.log("window closed...");
            });

    });
};

ElectronWindow.prototype.ReloadWindow = function () {
    $(document).ready(function () {
        $.ajax({
            url: "/home/ReloadWindow"
        }).done(function () {
            console.log("window reloaded...");
        });

    });
};

ElectronWindow.prototype.OpenDevTools = function () {
    $(document).ready(function () {
        $.ajax({
            url: "/home/OpenDevTools"
        }).done(function () {
            console.log("opened dev tools...");
        });

    });
};