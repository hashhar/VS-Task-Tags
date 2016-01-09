using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Task_Tags_Manager
{
	/// <summary>
	/// Highlighter places red boxes behind all the "TODO"s in the editor window
	/// </summary>
	internal sealed class Highlighter
	{
		/// <summary>
		/// The layer of the adornment.
		/// </summary>
		private readonly IAdornmentLayer _adornmentLayer;

		/// <summary>
		/// Text view where the adornment is created.
		/// </summary>
		private readonly IWpfTextView _wpfTextView;

		/// <summary>
		/// Adornment brush.
		/// </summary>
		private readonly Brush _brush;

		/// <summary>
		/// Adornment pen.
		/// </summary>
		private readonly Pen _pen;

		/// <summary>
		/// Initializes a new instance of the <see cref="Highlighter"/> class.
		/// </summary>
		/// <param name="wpfTextView">Text view to create the adornment for</param>
		public Highlighter(IWpfTextView wpfTextView)
		{
			if (wpfTextView == null)
			{
				throw new ArgumentNullException(nameof(wpfTextView));
			}

			_adornmentLayer = wpfTextView.GetAdornmentLayer("Highlighter");

			_wpfTextView = wpfTextView;
			// Listen to any event that changes the layout (text changes, scrolling, etc)
			_wpfTextView.LayoutChanged += OnLayoutChanged;

			// Create the pen and brush to color the box behind the TODO's
			_brush = new SolidColorBrush(Color.FromArgb(63, 221, 255, 0));
			_brush.Freeze();

			var penBrush = new SolidColorBrush(Color.FromArgb(255, 221, 255, 0));
			penBrush.Freeze();
			_pen = new Pen(penBrush, 0.5);
			_pen.Freeze();
		}

		/// <summary>
		/// Handles whenever the text displayed in the view changes by adding the adornment to any reformatted lines
		/// </summary>
		/// <remarks><para>This event is raised whenever the rendered text displayed in the <see cref="ITextView"/> changes.</para>
		/// <para>It is raised whenever the view does a layout (which happens when DisplayTextLineContainingBufferPosition is called or in response to text or classification changes).</para>
		/// <para>It is also raised whenever the view scrolls horizontally or when its size changes.</para>
		/// </remarks>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
		{
			foreach (var line in e.NewOrReformattedLines)
			{
				CreateVisuals();
			}
		}

		/// <summary>
		/// Adds the adornment behind the "TODO"s within the given line
		/// </summary>
		private void CreateVisuals()
		{
			var textViewLines = _wpfTextView.TextViewLines;
			var text = textViewLines.FormattedSpan.Snapshot.GetText();
			var todoRegex = new Regex(@"\/\/\s*TODO\b");
			var match = todoRegex.Match(text);
			while (match.Success)
			{
				var matchStart = match.Index;
				var span = new SnapshotSpan(_wpfTextView.TextSnapshot, Span.FromBounds(matchStart, matchStart + match.Length));
				DrawAdornment(textViewLines, span);
				match = match.NextMatch();
			}
			/*
			// Loop through each character
			for (int charIndex = line.Start; charIndex < line.End; charIndex++)
			{
				// Check if the current letter is 'T' and the buffer is large enough so that a "TODO" may exist
				if (_wpfTextView.TextSnapshot[charIndex] == 'T' && charIndex + 4 < line.End)
				{
					// Get a string of 4 characters starting from the 'T'
					string snapshot = _wpfTextView.TextSnapshot.GetText(charIndex, 4);
					// Is the string a TODO?
					if (snapshot.Equals("TODO"))
					{
						SnapshotSpan span = new SnapshotSpan(_wpfTextView.TextSnapshot, Span.FromBounds(charIndex, charIndex + 4));
						DrawAdornment(textViewLines, span);
					}
				}
			}
			*/
		}

		private void DrawAdornment(IWpfTextViewLineCollection textViewLines, SnapshotSpan span)
		{
			var geometry = textViewLines.GetMarkerGeometry(span);
			if (geometry != null)
			{
				var drawing = new GeometryDrawing(_brush, _pen, geometry);
				drawing.Freeze();

				var drawingImage = new DrawingImage(drawing);
				drawingImage.Freeze();

				var image = new Image
				{
					Source = drawingImage,
				};

				// Align the image with the top of the bounds of the text geometry
				Canvas.SetLeft(image, geometry.Bounds.Left);
				Canvas.SetTop(image, geometry.Bounds.Top);

				_adornmentLayer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, image, null);
			}
		}
	}
}
