// Copyright 2016 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific
// language governing permissions and limitations under the License.

using System;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.UI.Controls;
using Foundation;
using UIKit;

namespace ArcGISRuntime.Samples.DisplayDrawingStatus
{
    [Register("DisplayDrawingStatus")]
    [ArcGISRuntime.Samples.Shared.Attributes.Sample(
        "Display drawing status",
        "MapView",
        "This sample demonstrates how to use the DrawStatus value of the MapView to notify user that the MapView is drawing.",
        "")]
    public class DisplayDrawingStatus : UIViewController
    {
        // Hold references to the UI controls.
        private MapView _myMapView;
        private UIActivityIndicatorView _activityIndicator;

        public DisplayDrawingStatus()
        {
            Title = "Display drawing status";
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Initialize();
        }

        public override void LoadView()
        {
            _myMapView = new MapView();
            _myMapView.TranslatesAutoresizingMaskIntoConstraints = false;

            _activityIndicator = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge)
            {
                BackgroundColor = UIColor.FromWhiteAlpha(0, .6f),
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            View = new UIView();
            View.AddSubviews(_myMapView, _activityIndicator);

            _myMapView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor).Active = true;
            _myMapView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor).Active = true;
            _myMapView.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor).Active = true;
            _myMapView.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor).Active = true;

            _activityIndicator.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor).Active = true;
            _activityIndicator.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor).Active = true;
            _activityIndicator.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor).Active = true;
            _activityIndicator.HeightAnchor.ConstraintEqualTo(40).Active = true;
        }

        private async void Initialize()
        {
            // Create new Map with basemap.
            Map myMap = new Map(BasemapType.Topographic, 34.056, -117.196, 4);

            // URL to the feature service.
            Uri serviceUri = new Uri("https://sampleserver6.arcgisonline.com/arcgis/rest/services/DamageAssessment/FeatureServer/0");

            // Initialize a new feature layer.
            ServiceFeatureTable myFeatureTable = new ServiceFeatureTable(serviceUri);
            FeatureLayer myFeatureLayer = new FeatureLayer(myFeatureTable);

            // Load the feature layer.
            await myFeatureLayer.LoadAsync();

            // Add the feature layer to the Map.
            myMap.OperationalLayers.Add(myFeatureLayer);

            // Provide used Map to the MapView.
            _myMapView.Map = myMap;

            // Hook up the DrawStatusChanged event.
            _myMapView.DrawStatusChanged += OnMapViewDrawStatusChanged;

            // Animate the activity spinner.
            _activityIndicator.StartAnimating();
        }

        private void OnMapViewDrawStatusChanged(object sender, DrawStatusChangedEventArgs e)
        {
            // Make sure that the UI changes are done in the UI thread.
            BeginInvokeOnMainThread(() =>
            {
                // Show the activity indicator if the map is drawing.
                if (e.Status == DrawStatus.InProgress)
                {
                    _activityIndicator.Hidden = false;
                }
                else
                {
                    _activityIndicator.Hidden = true;
                }
            });
        }
    }
}