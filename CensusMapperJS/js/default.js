// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509
(function () {
    "use strict";

    WinJS.Binding.optimizeBindingReferences = true;

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;

    app.onactivated = function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                // TODO: This application has been newly launched. Initialize
                // your application here.
            } else {
                // TODO: This application has been reactivated from suspension.
                // Restore application state here.
            }
            args.setPromise(WinJS.UI.processAll());
        }
    };

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. You might use the
        // WinJS.Application.sessionState object, which is automatically
        // saved and restored across suspension. If you need to complete an
        // asynchronous operation before your application is suspended, call
        // args.setPromise().
    };

    app.start();
    
    function initialize() {
        Microsoft.Maps.loadModule('Microsoft.Maps.Map', { callback: initMap, culture: 'en-us', homeRegion: 'US' });
    }
    document.addEventListener("DOMContentLoaded", initialize, false);
    
    var map;
    function initMap() {
        try {
            var mapOptions =
            {
                // Add your Bing Maps key here
                credentials: 'AokHTH0-W5lExUUHR15VLgxjPm-OhzFMeNM5tqpgvAxy4TtvxKmsW6JekmllB3m5',
                center: new Microsoft.Maps.Location(40.71, -74.00),
                mapTypeId: Microsoft.Maps.MapTypeId.road,
                zoom: 8
            };
            map = new Microsoft.Maps.Map(document.getElementById("mapdiv"), mapOptions);
        }
        catch (e) {
            var md = new Windows.UI.Popups.MessageDialog(e.message);
            md.showAsync();
        }
    }
})();
