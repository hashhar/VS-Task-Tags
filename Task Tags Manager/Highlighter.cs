using System;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace Task_Tags_Manager
{
	/// <summary>
	/// Highlighter places red boxes behind all the "a"s in the editor window
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
				throw new ArgumentNullException("wpfTextView");
			}

			_adornmentLayer = wpfTextView.GetAdornmentLayer("Highlighter");

			_wpfTextView = wpfTextView;
			_wpfTextView.LayoutChanged += OnLayoutChanged;

			// Create the pen and brush to color the box behind the a's
			_brush = new SolidColorBrush(Color.FromArgb(0x20, 0x00, 0x00, 0xff));
			_brush.Freeze();

			var penBrush = new SolidColorBrush(Colors.Red);
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
		internal void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
		{
			foreach (ITextViewLine line in e.NewOrReformattedLines)
			{
				CreateVisuals(line);
			}
		}

		/// <summary>
		/// Adds the scarlet box behind the 'a' characters within the given line
		/// </summary>
		/// <param name="line">Line to add the adornments</param>
		private void CreateVisuals(ITextViewLine line)
		{
			IWpfTextViewLineCollection textViewLines = _wpfTextView.TextViewLines;

			// Loop through each character, and place a box around any 'a'
			for (int charIndex = line.Start; charIndex < line.End; charIndex++)
			{
				if (_wpfTextView.TextSnapshot[charIndex] == 'a')
				{
					SnapshotSpan span = new SnapshotSpan(_wpfTextView.TextSnapshot, Span.FromBounds(charIndex, charIndex + 1));
					Geometry geometry = textViewLines.GetMarkerGeometry(span);
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
	}
}
