using System.Drawing;

namespace ImageFilterBack
{
    public delegate void ProgressDelegate(double percent);

    public interface ImageHandler
    {
        string HandlerName { get; }
        void init(SortedList<string, object> parameters);
        Bitmap Source { set; }
        Bitmap Result { get; }
        void startHandle(ProgressDelegate progress);
    }

    public class ImageFilter : ImageHandler
    {
        private Bitmap source;
        private Bitmap result;
        public string HandlerName
        {
            get => "Фильтр Canny";
        }
        public void init(SortedList<string, object> parameters)
        {
            throw new NotImplementedException();
        }
        public Bitmap Source { set => source = value; }
        public Bitmap Result { get => result; }
        public void startHandle(ProgressDelegate progress)
        {
            result = (Bitmap)source.Clone();
            Color oldColor, newColor;
            

            // Creating a grayscale version of the image
            Bitmap grayscale = new Bitmap(result.Width, result.Height);

            for (int x = 0; x < result.Width; x++)
            {
                for (int y = 0; y < result.Height; y++)
                {
                    Color color = source.GetPixel(x, y);
                    int grey = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
                    grayscale.SetPixel(x, y, Color.FromArgb(grey, grey, grey));
                }
            }

            // Apply the Canny filter
            Bitmap result2 = new Bitmap(source.Width, source.Height);
            int[,] gx = { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            int[,] gy = { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } };
            double[,] values = new double[result2.Width, result2.Height];
            double[,] angles = new double[result2.Width, result2.Height];

            for (int x = 1; x < source.Width - 1; x++)
            {
                for (int y = 1; y < source.Height - 1; y++)
                {
                    double xGradient = 0;
                    double yGradient = 0;

                    // Calculate the gradients of x and y using the Sobel operator
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            int grey = grayscale.GetPixel(x + i, y + j).R;
                            xGradient += gx[i + 1, j + 1] * grey;
                            yGradient += gy[i + 1, j + 1] * grey;
                        }
                    }

                    // Calculating the magnitude and angle of the gradient
                    double value = Math.Sqrt(xGradient * xGradient + yGradient * yGradient);
                    double angle = Math.Atan2(yGradient, xGradient);

                    values[x, y] = value;
                    angles[x, y] = angle;
                }
            }

            for (int x = 1; x < source.Width - 1; x++)
            {
                for (int y = 1; y < source.Height - 1; y++)
                {
                    double angle = angles[x, y];
                    double value = values[x, y];
                    double number = 255;

                    // Non-maximal suppression
                    if ((angle < -Math.PI / 8 || angle >= Math.PI / 8) && value < values[x - 1, y] && value < values[x + 1, y])
                    {
                        number = 0;
                    }
                    else if ((angle >= -Math.PI / 8 && angle < Math.PI / 8) && value < values[x, y - 1] && value < values[x, y + 1])
                    {
                        number = 0;
                    }
                    else if ((angle >= Math.PI / 8 && angle < 3 * Math.PI / 8) && value < values[x - 1, y - 1] && value < values[x + 1, y + 1])
                    {
                        number = 0;
                    }
                    else if ((angle >= -3 * Math.PI / 8 && angle < -Math.PI / 8) && value < values[x + 1, y - 1] && value < values[x - 1, y + 1])
                    {
                        number = 0;
                    }

                    // hysteresis threshold value
                    if (number != 0 && value < 50)
                    {
                        number = 0;
                    }
                    else if (number != 255 && value >= 100)
                    {
                        number = 255;
                    }

                    result.SetPixel(x, y, Color.FromArgb((int)number, (int)number, (int)number));
                    
                }
                progress(1);
            }
            

        }
    }
}
