﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows;

namespace MinecraftTextBlock_for_dotNET_Core
{
	public class MinecraftTextBlock : FrameworkElement
	{
		private const string RANDOM_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
		private const string OBFUSCATION_STOPPERS = "0123456789abcdefrABCDEFR";
		private static readonly Random Random = new Random();

		private readonly DispatcherTimer _timer;

		private FormattedText _formattedText;
		private bool _isAnimated;
		private bool IsAnimated
		{
			get { return _isAnimated; }
			set
			{
				_isAnimated = value;

				if (_timer != null)
				{
					if (value)
						_timer.Start();
					else
						_timer.Stop();
				}
			}
		}

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public static readonly DependencyProperty TextProperty =
					DependencyProperty.Register("Text", typeof(string), typeof(MinecraftTextBlock), new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.AffectsArrange, OnTextChanged));

		public FontFamily FontFamily
		{
			get { return (FontFamily)GetValue(FontFamilyProperty); }
			set { SetValue(FontFamilyProperty, value); }
		}
		public static readonly DependencyProperty FontFamilyProperty =
					DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(MinecraftTextBlock), new FrameworkPropertyMetadata(SystemFonts.MessageFontFamily, FrameworkPropertyMetadataOptions.AffectsMeasure, OnTextChanged));

		public double FontSize
		{
			get { return (double)GetValue(FontSizeProperty); }
			set { SetValue(FontSizeProperty, value); }
		}
		public static readonly DependencyProperty FontSizeProperty =
					DependencyProperty.Register("FontSize", typeof(double), typeof(MinecraftTextBlock), new FrameworkPropertyMetadata(12d, FrameworkPropertyMetadataOptions.AffectsMeasure, OnTextChanged));

		public Color Foreground
		{
			get { return (Color)GetValue(ForegroundProperty); }
			set { SetValue(ForegroundProperty, value); }
		}
		public static readonly DependencyProperty ForegroundProperty =
					DependencyProperty.Register("Foreground", typeof(Color), typeof(MinecraftTextBlock), new FrameworkPropertyMetadata(SystemColors.InfoTextColor, FrameworkPropertyMetadataOptions.AffectsRender, OnTextChanged));

		public TextAlignment TextAlignment
		{
			get { return (TextAlignment)GetValue(TextAlignmentProperty); }
			set { SetValue(TextAlignmentProperty, value); }
		}
		public static readonly DependencyProperty TextAlignmentProperty =
					DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(MinecraftTextBlock), new FrameworkPropertyMetadata(TextAlignment.Left, FrameworkPropertyMetadataOptions.AffectsMeasure, OnTextChanged));

		public MinecraftTextBlock()
		{
			_timer = new DispatcherTimer(TimeSpan.FromSeconds(1 / 30d), DispatcherPriority.Render, (e, s) =>
			{
				_formattedText = null;
				InvalidateMeasure();
				InvalidateVisual();
			}, Dispatcher);
		}

		private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var mcText = d as MinecraftTextBlock;
			if (mcText == null) return;

			mcText._formattedText = null;
			mcText.InvalidateMeasure();
			mcText.InvalidateVisual();
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			EnsureFormattedText();
			_formattedText.SetFontSize(FontSize);
			Width = _formattedText.Width + 2;
			Height = _formattedText.Height * 1.1;
			if (VerticalAlignment == VerticalAlignment.Center) drawingContext.DrawText(_formattedText, new Point(0, (Height - _formattedText.Height) / 2));
			else drawingContext.DrawText(_formattedText, new Point(0, 0));
			base.OnRender(drawingContext);
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			EnsureFormattedText();

			// constrain the formatted text according to the available size
			// the Math.Min call is important - without this constraint (which seems arbitrary, but is the maximum allowable text width),
			// things blow up when availableSize is infinite in both directions
			// the Math.Max call is to ensure we don't hit zero, which will cause MaxTextHeight to throw
			//_formattedText.MaxTextWidth = Math.Min(3579139, availableSize.Width);
			//_formattedText.MaxTextHeight = Math.Max(0.0001d, availableSize.Height);

			// return the desired size
			return new Size(_formattedText.Width, _formattedText.Height);
		}

		private void EnsureFormattedText()
		{
			if (_formattedText != null) return;

			IsAnimated = false; //Wird in RemoveCodes() auf true gesetzt, falls irgendwas tatsächlcih effektiv obfuscated ist
			_formattedText = new FormattedText(
				RemoveCodes(Text),
				CultureInfo.CurrentCulture,
				FlowDirection.LeftToRight,
				new Typeface(FontFamily,
				FontStyles.Normal,
				FontWeights.Normal,
				FontStretches.Normal),
				FontSize,
				new SolidColorBrush(Foreground),
				VisualTreeHelper.GetDpi(this).PixelsPerDip)
			{
				TextAlignment = TextAlignment
			};
			var currentColor = Foreground;
			var currentStyle = FontStyles.Normal;
			var currentWeight = FontWeights.Normal;
			var currentDecorations = new TextDecorationCollection();
			var lastPos = 0;
			var i = 0;

			foreach (Match m in Formatting.MinecraftFormattings.Matches(Text))
			{
				i++;
				var realIndex = m.Index - i * 2 + 2;
				_formattedText.SetForegroundBrush(new SolidColorBrush(currentColor), lastPos, realIndex - lastPos);
				_formattedText.SetFontStyle(currentStyle, lastPos, realIndex - lastPos);
				_formattedText.SetFontWeight(currentWeight, lastPos, realIndex - lastPos);
				_formattedText.SetTextDecorations(currentDecorations.CloneCurrentValue(), lastPos, realIndex - lastPos);

				var c = m.Groups[1].Value[0];
				switch (c)
				{
					case 'r':
					case 'R':
						currentColor = Foreground;
						currentStyle = FontStyles.Normal;
						currentWeight = FontWeights.Normal;
						currentDecorations.Clear();
						break;
					case 'l':
					case 'L':
						currentWeight = FontWeights.Bold;
						break;
					case 'o':
					case 'O':
						currentStyle = FontStyles.Italic;
						break;
					case 'm':
					case 'M':
						currentDecorations.Add(TextDecorations.Strikethrough);
						break;
					case 'n':
					case 'N':
						currentDecorations.Add(TextDecorations.Underline);
						break;
					default:
						currentColor = MinecraftColors.FromChar(c) ?? currentColor;

						//Color codes reset the current formatting!
						currentStyle = FontStyles.Normal;
						currentWeight = FontWeights.Normal;
						currentDecorations.Clear();
						break;
				}
				lastPos = realIndex;
			}
			_formattedText.SetForegroundBrush(new SolidColorBrush(currentColor), lastPos, _formattedText.Text.Length - lastPos);
			_formattedText.SetFontStyle(currentStyle, lastPos, _formattedText.Text.Length - lastPos);
			_formattedText.SetFontWeight(currentWeight, lastPos, _formattedText.Text.Length - lastPos);
			_formattedText.SetTextDecorations(currentDecorations.CloneCurrentValue(), lastPos, _formattedText.Text.Length - lastPos);
		}

		/// <summary>
		/// Entfernt alle Minecraft-Formatierungszeichen aus dem angegebenen String und ersetzt ggf.
		/// </summary>
		/// <param name="original"></param>
		/// <returns></returns>
		private string RemoveCodes(string original)
		{
			var sb = new StringBuilder();
			var wasParagraph = false;
			var isObfuscated = false;

			foreach (var c in original)
			{
				if (wasParagraph)
				{
					if (c == 'k' || c == 'K')
					{
						isObfuscated = true;
						IsAnimated = true;
					}
					else
					{
						if (OBFUSCATION_STOPPERS.Contains(c))
							isObfuscated = false;
						sb.Append('§').Append(c);
					}
					wasParagraph = false;
				}
				else
				{
					if (c == '§')
						wasParagraph = true;
					else
						sb.Append(isObfuscated ? GetRandomChar() : c);
				}
			}

			return Formatting.MinecraftFormattings.Replace(sb.ToString(), String.Empty);
		}

		private char GetRandomChar()
		{
			return RANDOM_CHARS[Random.Next(RANDOM_CHARS.Length)];
		}
	}
}

