namespace BingMapMVVM
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using Bing.Maps;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using System.Collections;
    using System;
    using Windows.Foundation;
    using System.ComponentModel;
    using Windows.UI.Xaml.Data;

    public sealed class BindableMap : Control
    {
        private readonly Dictionary<object, FrameworkElement> _childControls;

        private Map _mapControl;
        private bool _inited = false;

        public BindableMap()
        {
            this.DefaultStyleKey = typeof(BindableMap);
            _childControls = new Dictionary<object, FrameworkElement>();
        }

        public ObservableCollection<MapShapeLayer> ShapeLayers
        {
            get { return (ObservableCollection<MapShapeLayer>)GetValue(ShapeLayersProperty); }
            set { SetValue(ShapeLayersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShapeLayerCollection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShapeLayersProperty =
            DependencyProperty.Register("ShapeLayers", typeof(ObservableCollection<MapShapeLayer>), typeof(BindableMap), new PropertyMetadata(null, OnShapeLayersPropertyChanged));

        private static void OnShapeLayersPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventAgrs)
        {
            ((BindableMap)sender).OnShapeLayersPropertyChanged((ICollection<MapShapeLayer>)eventAgrs.OldValue, (ICollection<MapShapeLayer>)eventAgrs.NewValue);
        }

        public ICollection<MapTileLayer> TileLayers
        {
            get { return (ICollection<MapTileLayer>)GetValue(TileLayersProperty); }
            set { SetValue(TileLayersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TileLayer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TileLayersProperty =
            DependencyProperty.Register("TileLayers", typeof(ICollection<MapTileLayer>), typeof(BindableMap), new PropertyMetadata(null, OnTileLayersPropertyChanged));

        private static void OnTileLayersPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventAgrs)
        {
            ((BindableMap)sender).OnTileLayersPropertyChanged((ICollection<MapTileLayer>)eventAgrs.OldValue, (ICollection<MapTileLayer>)eventAgrs.NewValue);
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(BindableMap), new PropertyMetadata(null, OnItemSourcePropertyChanged));

        private static void OnItemSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventAgrs)
        {
            ((BindableMap)sender).OnItemSourcePropertyChanged((IEnumerable)eventAgrs.OldValue, (IEnumerable)eventAgrs.NewValue);
        }



        public DataTemplateSelector DataTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(DataTemplateSelectorProperty); }
            set { SetValue(DataTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataTemplateSelectorProperty =
            DependencyProperty.Register("DataTemplateSelector", typeof(DataTemplateSelector), typeof(BindableMap), new PropertyMetadata(null));





        public string Credentials
        {
            get { return (string)GetValue(CredentialsProperty); }
            set { SetValue(CredentialsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Credentials.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CredentialsProperty =
            DependencyProperty.Register("Credentials", typeof(string), typeof(BindableMap), new PropertyMetadata(string.Empty, OnCredentialsPropertyChanged));

        private static void OnCredentialsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            ((BindableMap)sender).OnCredentialsPropertyChanged((string)eventArgs.OldValue, (string)eventArgs.NewValue);
        }

        public double ZoomLevel
        {
            get { return (double)GetValue(ZoomLevelProperty); }
            set { SetValue(ZoomLevelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomLevel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomLevelProperty =
            DependencyProperty.Register("ZoomLevel", typeof(double), typeof(BindableMap), new PropertyMetadata(1d, OnZoomLevelPropertyChanged));

        private static void OnZoomLevelPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            ((BindableMap)sender).OnZoomLevelPropertyChanged((double)eventArgs.OldValue, (double)eventArgs.NewValue);
        }

        public MapType MapType
        {
            get { return (MapType)GetValue(MapTypeProperty); }
            set { SetValue(MapTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MapType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapTypeProperty =
            DependencyProperty.Register("MapType", typeof(MapType), typeof(BindableMap), new PropertyMetadata(MapType.Road, OnMapTypePropertyChanged));

        private static void OnMapTypePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            ((BindableMap)sender).OnMapTypePropertyChanged((MapType)eventArgs.OldValue, (MapType)eventArgs.NewValue);
        }

        public Location Center
        {
            get { return (Location)GetValue(CenterProperty); }
            set { SetValue(CenterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Center.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register("Center", typeof(Location), typeof(BindableMap), new PropertyMetadata(null, OnCenterPropertyChanged));

        private static void OnCenterPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            ((BindableMap)sender).OnCenterPropertyChanged((Location)eventArgs.OldValue, (Location)eventArgs.NewValue);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _mapControl = (Map)this.GetTemplateChild("PART_Map");

            ApplyCredentials();

            ZoomLevel = _mapControl.ZoomLevel;
            ApplyZoomLevel();

            MapType = _mapControl.MapType;
            ApplyMapType();

            Center = _mapControl.Center;
            ApplyCenter();

            ApplyShapeLayerCollection();
            ApplyTileLayerCollection();
            PopulateItems();


            _inited = true;
        }

        private void ApplyShapeLayerCollection()
        {
            var collection = ShapeLayers;
            if (collection != null)
            {
                foreach (var item in collection.ToList())
                {
                    _mapControl.ShapeLayers.Add(item);
                }
            }
        }

        private void ApplyTileLayerCollection()
        {
            var collection = TileLayers;
            if (collection != null)
            {
                foreach (var item in collection.ToList())
                {
                    _mapControl.TileLayers.Add(item);
                }
            }
        }

        private void ApplyCredentials()
        {
            _mapControl.Credentials = Credentials;
        }

        private void ApplyZoomLevel()
        {
            _mapControl.ZoomLevel = ZoomLevel;
        }

        private void ApplyMapType()
        {
            _mapControl.MapType = MapType;
        }

        private void ApplyCenter()
        {
            _mapControl.Center = Center;
        }

        private void OnShapeLayersPropertyChanged(ICollection<MapShapeLayer> oldValue, ICollection<MapShapeLayer> newValue)
        {
            if (!_inited)
                return;

            INotifyCollectionChanged oldCollectionChanged = oldValue as INotifyCollectionChanged;
            if (oldCollectionChanged != null)
            {
                oldCollectionChanged.CollectionChanged -= ShapeLayer_CollectionChanged;
            }

            _mapControl.ShapeLayers.Clear();
            ApplyShapeLayerCollection();

            INotifyCollectionChanged newCollectionChanged = newValue as INotifyCollectionChanged;
            if (newCollectionChanged != null)
            {
                newCollectionChanged.CollectionChanged += ShapeLayer_CollectionChanged;
            }
        }

        private void OnTileLayersPropertyChanged(ICollection<MapTileLayer> oldValue, ICollection<MapTileLayer> newValue)
        {
            if (!_inited)
                return;

            INotifyCollectionChanged oldCollectionChanged = oldValue as INotifyCollectionChanged;
            if (oldCollectionChanged != null)
            {
                oldCollectionChanged.CollectionChanged -= TileLayer_CollectionChanged;
            }

            _mapControl.ShapeLayers.Clear();
            ApplyShapeLayerCollection();

            INotifyCollectionChanged newCollectionChanged = newValue as INotifyCollectionChanged;
            if (newCollectionChanged != null)
            {
                newCollectionChanged.CollectionChanged += TileLayer_CollectionChanged;
            }
        }

        private void OnItemSourcePropertyChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            INotifyCollectionChanged oldCollectionChanged = oldValue as INotifyCollectionChanged;
            if (oldCollectionChanged != null)
                oldCollectionChanged.CollectionChanged -= ItemSource_CollectionChanged;

            if (_inited)
            {
                ClearItems();
                PopulateItems();
            }

            INotifyCollectionChanged newCollectionChanged = newValue as INotifyCollectionChanged;
            if (newCollectionChanged != null)
                newCollectionChanged.CollectionChanged += ItemSource_CollectionChanged;
        }



        private void OnCredentialsPropertyChanged(string oldValue, string newValue)
        {
            if (!_inited)
                return;

            ApplyCredentials();
        }

        private void OnZoomLevelPropertyChanged(double oldValue, double newValue)
        {
            if (!_inited)
                return;

            ApplyZoomLevel();
        }

        private void OnMapTypePropertyChanged(Bing.Maps.MapType oldValue, Bing.Maps.MapType newValue)
        {
            if (!_inited)
                return;

            ApplyMapType();
        }

        private void OnCenterPropertyChanged(Location oldValue, Location newValue)
        {
            if (_inited)
                return;

            ApplyCenter();
        }

        private void ShapeLayer_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (MapShapeLayer layer in e.NewItems)
                    _mapControl.ShapeLayers.Add(layer);
            }

            if (e.OldItems != null)
            {
                foreach (MapShapeLayer layer in e.OldItems)
                    _mapControl.ShapeLayers.Remove(layer);
            }
        }

        private void TileLayer_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (MapTileLayer layer in e.NewItems)
                    _mapControl.TileLayers.Add(layer);
            }

            if (e.OldItems != null)
            {
                foreach (MapTileLayer layer in e.OldItems)
                    _mapControl.TileLayers.Remove(layer);
            }
        }

        private void ItemSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!_inited)
                return;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                    {
                        foreach (var item in e.NewItems)
                            AddItem(DataTemplateSelector, item);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems)
                        {
                            var uiControl = _childControls[item];
                            _mapControl.Children.Remove(uiControl);
                            _childControls.Remove(item);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    ClearItems();
                    PopulateItems();
                    break;
            }
        }

        private void ClearItems()
        {
            lock (_childControls)
            {
                foreach (var item in _childControls.Values.ToArray())
                    _mapControl.Children.Remove(item);

                _childControls.Clear();
            }
        }

        private void PopulateItems()
        {
            var items = ItemsSource;
            var datatemplateSelector = DataTemplateSelector;

            if (items != null && datatemplateSelector == null)
                throw new Exception("DataTemplate selector required!");

            lock (_childControls)
            {
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        AddItem(datatemplateSelector, item);
                    }
                }
            }
        }

        private void AddItem(Windows.UI.Xaml.Controls.DataTemplateSelector datatemplateSelector, object item)
        {
            var presenter = new MapItem();
            presenter.RenderTransformOrigin = new Point(.5, .5);
            presenter.DataContext = item;

            DataTemplate dataTemplate = (DataTemplate)datatemplateSelector.SelectTemplate(item, presenter);
            presenter.ContentTemplate = dataTemplate;


            _childControls.Add(item, presenter);
            _mapControl.Children.Add(presenter);

            var latitudeBinding =  new Binding 
            { 
                Path = new PropertyPath(LocationAwareDataTemplate.GetLatitudePath(dataTemplate))
            };
            presenter.SetBinding(MapItem.LatitudeProperty, latitudeBinding);

            var longitudeBinding = new Binding
            {
                Path = new PropertyPath(LocationAwareDataTemplate.GetLongitudePath(dataTemplate))
            };
            presenter.SetBinding(MapItem.LongitudeProperty, longitudeBinding);
        }


        public bool TryPixelToLocation(Point pos, out Location location)
        {
            return _mapControl.TryPixelToLocation(pos, out location);
        }
    }
}
