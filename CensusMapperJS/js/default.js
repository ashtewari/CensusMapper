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
        // init bing maps
        Microsoft.Maps.loadModule('Microsoft.Maps.Map', { callback: initMap, culture: 'en-us', homeRegion: 'US' });

        //hook button clicks
        WinJS.UI.processAll().done(function () {
            var cmdRefresh = document.getElementById("cmdRefresh");
            cmdRefresh.addEventListener("click", function() {
                // get census data and display it              

                map.entities.clear();
                
                var pkg = Windows.ApplicationModel.Package.current;
                var folder = pkg.installedLocation;
                folder.getFileAsync("ApiKeys.json").done(function(file) {
                    Windows.Storage.FileIO.readTextAsync(file).done(function(text) {
                        var censusKey = JSON.parse(text).Census;
                        
                        var options = {
                            url: "http://api.census.gov/data/2010/sf1?key=" + censusKey + "&get=P0010001&for=state:*",
                            type: "GET",
                        };

                        folder.getFileAsync("\data\\states-data.json").done(function(statesfile) {
                            Windows.Storage.FileIO.readTextAsync(statesfile).done(function(statesData) {
                                console.log(statesData);
                                var states = JSON.parse(statesData);

                                WinJS.xhr(options).done(
                                    function success(req) {
                                        var data = JSON.parse(req.responseText);
                                        for (var i = 0; i < data.length; i++) {
                                            console.log(data[i][0] + " " + data[i][1]);
                                            for (var j = 0; j < states.length; j++) {
                                                if (data[i][1] == states[j].id) {
                                                    console.log(states[j].name);                                                    
                                                    var pushpinOptions = { width: null, height: null, htmlContent: "<div class='population-box'><div class=state-name>" + states[j].name + "</div><div>" + data[i][0] + "</div></div>" };
                                                    var pushpin = new Microsoft.Maps.Pushpin(new Microsoft.Maps.Location(states[j].lat, states[j].lng), pushpinOptions);
                                                    map.entities.push(pushpin);
                                                }
                                            }
                                        }
                                    },
                                    function error(err) {
                                        var md = new Windows.UI.Popups.MessageDialog(err.message);
                                        md.showAsync();
                                    }
                                );
                            });
                        });
                    });
                });                             
            });
        });
    }
    document.addEventListener("DOMContentLoaded", initialize, false);
    
    var map;
    function initMap() {
        try {
            var pkg = Windows.ApplicationModel.Package.current;
            var folder = pkg.installedLocation;
            folder.getFileAsync("ApiKeys.json").done(function(file) {
                Windows.Storage.FileIO.readTextAsync(file).done(function(text) {
                    var bingKey = JSON.parse(text).Bing;

                    var mapOptions =
                    {
                        // Add your Bing Maps key here
                        credentials: bingKey,
                        center: new Microsoft.Maps.Location(39.833333, -98.583333),
                        mapTypeId: Microsoft.Maps.MapTypeId.road,
                        zoom: 5
                    };
                    map = new Microsoft.Maps.Map(document.getElementById("mapdiv"), mapOptions);
                });
            });
        }
        catch (e) {
            var md = new Windows.UI.Popups.MessageDialog(e.message);
            md.showAsync();
        }
    }
})();
