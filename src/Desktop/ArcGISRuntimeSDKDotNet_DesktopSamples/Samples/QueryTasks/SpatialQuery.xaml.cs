﻿using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks.Query;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace ArcGISRuntimeSDKDotNet_DesktopSamples.Samples
{
    /// <summary>
    /// This sample demonstrates how to spatially search your data using a QueryTask with its geometry attribute set. The sample spatially queries a map service using the buffer geometry around a user's click point and displays the results in a graphics layer on the map.
    /// </summary>
    /// <title>Spatial Query</title>
	/// <category>Tasks</category>
	/// <subcategory>Query</subcategory>
	public partial class SpatialQuery : UserControl
    {
        /// <summary>Construct Spatial Query sample control</summary>
        public SpatialQuery()
        {
            InitializeComponent();

			mapView.Map.InitialViewpoint = new Envelope(-9270434.248, 5246977.326, -9269261.417, 5247569.712);
            InitializePMS().ContinueWith((_) => { }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        // Initialize PushPin symbol
        private async Task InitializePMS()
        {
            try
            {
                var pms = layoutGrid.Resources["DefaultMarkerSymbol"] as PictureMarkerSymbol;
                await pms.SetSourceAsync(new Uri("pack://application:,,,/ArcGISRuntimeSDKDotNet_DesktopSamples;component/Assets/RedPushpin.png"));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        // buffer the click point, query the map service with the buffer geometry as the filter and add graphics to the map
        private async void mapView_MapViewTapped(object sender, Esri.ArcGISRuntime.Controls.MapViewInputEventArgs e)
        {
            try
            {
                graphicsLayer.Graphics.Add(new Graphic(e.Location));

                var bufferResult = GeometryEngine.Buffer(e.Location, 100);
                bufferLayer.Graphics.Add(new Graphic(bufferResult));

                var queryTask = new QueryTask(
                    new Uri("http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/BloomfieldHillsMichigan/Parcels/MapServer/2"));
                var query = new Query("1=1")
                {
                    ReturnGeometry = true,
                    OutSpatialReference = mapView.SpatialReference,
                    Geometry = bufferResult
                };
                query.OutFields.Add("OWNERNME1");

                var queryResult = await queryTask.ExecuteAsync(query);
                if (queryResult != null && queryResult.FeatureSet != null)
                {
                    parcelLayer.Graphics.AddRange(queryResult.FeatureSet.Features.OfType<Graphic>());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Spatial Query Sample");
            }
        }
    }
}