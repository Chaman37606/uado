﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Uno.AzureDevOps.Presentation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Uno.AzureDevOps.Views.Controls
{
	public enum NavigationLevel
	{
		Organizations,
		Projects,
		Project,
		Item
	}

	[SuppressMessage("", "SA1201", Justification = "Control properties")]
	[SuppressMessage("", "CA1720", Justification = "Control properties")]
	public sealed partial class SideMenu : UserControl
	{
		public SideMenu()
		{
			this.InitializeComponent();
			DataContext = new SideMenuViewModel();
		}

		public static readonly DependencyProperty NavigationLevelProperty = DependencyProperty.Register(
			"SourceUri",
			typeof(NavigationLevel?),
			typeof(SideMenu),
			new PropertyMetadata(default(NavigationLevel?), (d, e) => OnNavigationLevelChanged((SideMenu)d)));

		public Visibility? MenuVisibility
		{
			get => (Visibility?)GetValue(MenuVisibilityProperty);
			set => SetValue(MenuVisibilityProperty, value);
		}

		public static readonly DependencyProperty MenuVisibilityProperty =
			DependencyProperty.Register(
				nameof(MenuVisibility),
				typeof(Visibility?),
				typeof(SideMenu),
				new PropertyMetadata(null, MenuVisibilityChanged));

		private static void MenuVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is SideMenu menu && e.NewValue is Visibility visibility)
			{
#if !__WASM__
				if (visibility == Visibility.Visible)
				{
					menu.Animate(opacity: 1d, duration: 750);
				}
				else
				{
					menu.Animate(opacity: 0d, duration: 1);
				}
#endif
				menu.Visibility = visibility;
			}
		}

		public bool IsFullscreenMenu
		{
			get => (bool)GetValue(IsFullscreenMenuProperty);
			set => SetValue(IsFullscreenMenuProperty, value);
		}

		public static readonly DependencyProperty IsFullscreenMenuProperty =
			DependencyProperty.Register(
				nameof(IsFullscreenMenu),
				typeof(bool),
				typeof(SideMenu),
				new PropertyMetadata(true));

		public NavigationLevel? NavLevel
		{
			get => (NavigationLevel)GetValue(NavigationLevelProperty);
			set => SetValue(NavigationLevelProperty, value);
		}

		public void Animate(double opacity, double duration)
		{
			var stb = new Storyboard() { Duration = new Duration(TimeSpan.FromMilliseconds(duration)) };

			var logoFadeIn = new DoubleAnimation()
			{
				To = opacity,
				EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseInOut }
			};

			var topFadeIn = new DoubleAnimation()
			{
				To = opacity,
				EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseInOut }
			};

			var bottomFadeIn = new DoubleAnimation()
			{
				To = opacity,
				EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseInOut }
			};

			Storyboard.SetTarget(logoFadeIn, TopMenuLogoView);
			Storyboard.SetTargetProperty(logoFadeIn, "(UIElement.Opacity)");
			Storyboard.SetTarget(topFadeIn, TopMenuView);
			Storyboard.SetTargetProperty(topFadeIn, "(UIElement.Opacity)");
			Storyboard.SetTarget(bottomFadeIn, BottomMenuView);
			Storyboard.SetTargetProperty(bottomFadeIn, "(UIElement.Opacity)");

			stb.Children.Add(logoFadeIn);
			stb.Children.Add(topFadeIn);
			stb.Children.Add(bottomFadeIn);

			stb.Begin();
		}

		private static void OnNavigationLevelChanged(SideMenu sideMenu)
		{
			if (!sideMenu.NavLevel.HasValue)
			{
				return;
			}

			var button = sideMenu.ButtonAllProjects;
			var indicator = sideMenu.IndicatorButtonAllProjects;
			var currentNavigationLevel = sideMenu.NavLevel.Value;

			button.FontWeight = FontWeights.Normal;
			indicator.Visibility = Visibility.Collapsed;

			if (currentNavigationLevel == NavigationLevel.Organizations)
			{
				sideMenu.TopMenuLogoView.Visibility = Visibility.Visible;
				sideMenu.TopMenuView.Visibility = Visibility.Collapsed;
			}
			else if (currentNavigationLevel == NavigationLevel.Projects)
			{
				sideMenu.TopMenuLogoView.Visibility = Visibility.Collapsed;
				sideMenu.TopMenuView.Visibility = Visibility.Visible;
				button.FontWeight = FontWeights.Bold;
				indicator.Visibility = Visibility.Visible;
			}
			else if (currentNavigationLevel == NavigationLevel.Project || currentNavigationLevel == NavigationLevel.Item)
			{
				sideMenu.TopMenuLogoView.Visibility = Visibility.Collapsed;
				sideMenu.TopMenuView.Visibility = Visibility.Visible;
			}
		}
	}
}
