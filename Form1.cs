using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing; // Added for Point
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VideoGridProcessor
{
    public partial class Form1 : Form
    {
        private string outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "output");
        private string outputFilename;
        private string outputPath;

        // Variables for Drag-and-Drop Reordering
        private int indexOfItemUnderMouseToDrag;

        public Form1()
        {
            InitializeComponent();
            // Ensure output directory exists
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            // Enable drag-and-drop
            lstSelectedVideos.AllowDrop = true;
            lstSelectedVideos.MouseDown += new MouseEventHandler(lstSelectedVideos_MouseDown);
            lstSelectedVideos.DragOver += new DragEventHandler(lstSelectedVideos_DragOver);
            lstSelectedVideos.DragDrop += new DragEventHandler(lstSelectedVideos_DragDrop);
        }

        /// <summary>
        /// Event handler for the "Select Videos" button.
        /// Allows the user to select multiple video files.
        /// </summary>
        private void btnSelectVideos_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Multiselect = true;
                openFileDialog.Filter = "Video Files|*.mp4;*.mov;*.avi;*.mkv";
                openFileDialog.Title = "Select Video Files";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    lstSelectedVideos.Items.Clear();
                    foreach (string file in openFileDialog.FileNames)
                    {
                        lstSelectedVideos.Items.Add(file);
                    }
                }
            }
        }

        /// <summary>
        /// Event handler for the "Process Videos" button.
        /// Initiates the video processing using FFmpeg.
        /// </summary>
        private async void btnProcessVideos_Click(object sender, EventArgs e)
        {
            if (lstSelectedVideos.Items.Count < 2)
            {
                MessageBox.Show("Please select at least 2 video files.", "Insufficient Videos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<string> videoPaths = lstSelectedVideos.Items.Cast<string>().ToList();

            // Define output filename and path
            outputFilename = $"output_{DateTime.Now:yyyyMMddHHmmss}.mp4";
            outputPath = Path.Combine(outputDirectory, outputFilename);

            // Build filter_complex with drawtext overlays
            string filterComplex = BuildFilterComplex(videoPaths);
            if (string.IsNullOrEmpty(filterComplex))
            {
                MessageBox.Show("Unsupported number of videos. Please select between 2 and 6 videos.", "Unsupported Number of Videos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Build FFmpeg command for display
            string ffmpegCommand = BuildFFmpegCommand(videoPaths, filterComplex, outputPath);
            txtFFmpegCommand.Text = ffmpegCommand;

            // Disable buttons to prevent multiple operations
            btnSelectVideos.Enabled = false;
            btnProcessVideos.Enabled = false;
            btnMoveUp.Enabled = false;
            btnMoveDown.Enabled = false;

            // Reset progress bar and label
            progressBar.Value = 0;
            progressBar.Style = ProgressBarStyle.Blocks;
            lblOutputPath.Text = "Output will be saved to:";

            // Execute FFmpeg
            bool success = await ExecuteFFmpegAsync(ffmpegCommand);

            if (success)
            {
                lblOutputPath.Text = $"Output saved to: {outputPath}";
                MessageBox.Show("Video processing completed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                lblOutputPath.Text = "Output will be saved to:";
                MessageBox.Show("An error occurred during video processing.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Re-enable buttons
            btnSelectVideos.Enabled = true;
            btnProcessVideos.Enabled = true;
            btnMoveUp.Enabled = true;
            btnMoveDown.Enabled = true;
        }

        /// <summary>
        /// Builds the filter_complex string based on the number of videos.
        /// Supports 2 to 6 videos with drawtext overlays for visual order confirmation.
        /// </summary>
        /// <param name="videoPaths">List of video file paths.</param>
        /// <returns>filter_complex string.</returns>
        private string BuildFilterComplex(List<string> videoPaths)
        {
            int numVideos = videoPaths.Count;
            string filter = "";

            switch (numVideos)
            {
                case 2:
                    // Side by side with numbering
                    filter = "[0:v]scale=320:240,drawtext=text='1':x=10:y=10:fontsize=24:fontcolor=white[v0];" +
                             "[1:v]scale=320:240,drawtext=text='2':x=10:y=10:fontsize=24:fontcolor=white[v1];" +
                             "[v0][v1]hstack=inputs=2[out]";
                    break;
                case 3:
                    // Two on top, one below with numbering
                    filter = "[0:v]scale=320:240,drawtext=text='1':x=10:y=10:fontsize=24:fontcolor=white[v0];" +
                             "[1:v]scale=320:240,drawtext=text='2':x=10:y=10:fontsize=24:fontcolor=white[v1];" +
                             "[2:v]scale=640:240,drawtext=text='3':x=10:y=10:fontsize=24:fontcolor=white[v2];" +
                             "[v0][v1]hstack=inputs=2[top];" +
                             "[top][v2]vstack=inputs=2[out]";
                    break;
                case 4:
                    // 2x2 grid with numbering
                    filter = "[0:v]scale=320:240,drawtext=text='1':x=10:y=10:fontsize=24:fontcolor=white[v0];" +
                             "[1:v]scale=320:240,drawtext=text='2':x=10:y=10:fontsize=24:fontcolor=white[v1];" +
                             "[2:v]scale=320:240,drawtext=text='3':x=10:y=10:fontsize=24:fontcolor=white[v2];" +
                             "[3:v]scale=320:240,drawtext=text='4':x=10:y=10:fontsize=24:fontcolor=white[v3];" +
                             "[v0][v1]hstack=inputs=2[top];" +
                             "[v2][v3]hstack=inputs=2[bottom];" +
                             "[top][bottom]vstack=inputs=2[out]";
                    break;
                case 5:
                    // Three on top, two below with numbering
                    filter = "[0:v]scale=320:240,drawtext=text='1':x=10:y=10:fontsize=24:fontcolor=white[v0];" +
                             "[1:v]scale=320:240,drawtext=text='2':x=10:y=10:fontsize=24:fontcolor=white[v1];" +
                             "[2:v]scale=320:240,drawtext=text='3':x=10:y=10:fontsize=24:fontcolor=white[v2];" +
                             "[3:v]scale=320:240,drawtext=text='4':x=10:y=10:fontsize=24:fontcolor=white[v3];" +
                             "[4:v]scale=640:240,drawtext=text='5':x=10:y=10:fontsize=24:fontcolor=white[v4];" +
                             "[v0][v1][v2]hstack=inputs=3[top];" +
                             "[v3][v4]hstack=inputs=2[bottom];" +
                             "[top][bottom]vstack=inputs=2[out]";
                    break;
                case 6:
                    // 3x2 grid with numbering
                    filter = "[0:v]scale=320:240,drawtext=text='1':x=10:y=10:fontsize=24:fontcolor=white[v0];" +
                             "[1:v]scale=320:240,drawtext=text='2':x=10:y=10:fontsize=24:fontcolor=white[v1];" +
                             "[2:v]scale=320:240,drawtext=text='3':x=10:y=10:fontsize=24:fontcolor=white[v2];" +
                             "[3:v]scale=320:240,drawtext=text='4':x=10:y=10:fontsize=24:fontcolor=white[v3];" +
                             "[4:v]scale=320:240,drawtext=text='5':x=10:y=10:fontsize=24:fontcolor=white[v4];" +
                             "[5:v]scale=320:240,drawtext=text='6':x=10:y=10:fontsize=24:fontcolor=white[v5];" +
                             "[v0][v1][v2]hstack=inputs=3[top];" +
                             "[v3][v4][v5]hstack=inputs=3[bottom];" +
                             "[top][bottom]vstack=inputs=2[out]";
                    break;
                default:
                    // Unsupported number of videos
                    filter = "";
                    break;
            }

            return filter;
        }

        /// <summary>
        /// Builds the FFmpeg command string based on selected videos and filter_complex.
        /// </summary>
        /// <param name="videoPaths">List of video file paths.</param>
        /// <param name="filterComplex">filter_complex string.</param>
        /// <param name="outputPath">Output file path.</param>
        /// <returns>FFmpeg command string.</returns>
        private string BuildFFmpegCommand(List<string> videoPaths, string filterComplex, string outputPath)
        {
            string inputs = string.Join(" ", videoPaths.Select(p => $"-i \"{p}\""));
            string command = $"-y {inputs} -filter_complex \"{filterComplex}\" " +
                             $"-map \"[out]\" -c:v libx264 -preset fast \"{outputPath}\"";
            return command;
        }

        /// <summary>
        /// Executes the FFmpeg command asynchronously and updates the progress bar.
        /// </summary>
        /// <param name="ffmpegCommand">The FFmpeg command arguments.</param>
        /// <returns>True if successful, else false.</returns>
        private Task<bool> ExecuteFFmpegAsync(string ffmpegCommand)
        {
            return Task.Run(() =>
            {
                try
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = ffmpegCommand,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using (Process process = new Process())
                    {
                        process.StartInfo = startInfo;

                        // Subscribe to output and error events
                        process.ErrorDataReceived += new DataReceivedEventHandler(Process_ErrorDataReceived);
                        process.OutputDataReceived += new DataReceivedEventHandler(Process_OutputDataReceived);

                        process.Start();

                        process.BeginErrorReadLine();
                        process.BeginOutputReadLine();

                        process.WaitForExit();

                        return process.ExitCode == 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while executing FFmpeg:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            });
        }

        /// <summary>
        /// Event handler for FFmpeg's standard error output.
        /// Parses the output to update the progress bar.
        /// </summary>
        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data))
                return;

            // FFmpeg outputs progress information to stderr
            // Example line: frame=  120 fps= 25 q=28.0 size=    2048kB time=00:00:04.80 bitrate= 349.6kbits/s speed=1.25x
            // We can parse the 'time' field to estimate progress

            // Regex to extract time
            Regex timeRegex = new Regex(@"time=(\d+):(\d+):(\d+\.\d+)");
            Match match = timeRegex.Match(e.Data);
            if (match.Success)
            {
                int hours = int.Parse(match.Groups[1].Value);
                int minutes = int.Parse(match.Groups[2].Value);
                double seconds = double.Parse(match.Groups[3].Value);

                double currentTimeInSeconds = hours * 3600 + minutes * 60 + seconds;

                // To calculate progress, we need the total duration.
                // This requires querying FFmpeg for the duration of the first video.

                // For simplicity, we'll set progress to marquee style.
                // Implementing accurate progress requires more complex logic.

                // Invoke on UI thread
                this.Invoke(new Action(() =>
                {
                    if (progressBar.Style != ProgressBarStyle.Marquee)
                    {
                        progressBar.Style = ProgressBarStyle.Marquee;
                    }
                }));
            }
        }

        /// <summary>
        /// Event handler for FFmpeg's standard output.
        /// Can be used for logging or additional processing if needed.
        /// </summary>
        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            // Optional: Handle standard output if needed
        }

        /// <summary>
        /// Event handler for the "Move Up" button.
        /// Moves the selected video up in the list.
        /// </summary>
        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            int selectedIndex = lstSelectedVideos.SelectedIndex;
            if (selectedIndex > 0)
            {
                string selectedItem = lstSelectedVideos.SelectedItem.ToString();
                lstSelectedVideos.Items.RemoveAt(selectedIndex);
                lstSelectedVideos.Items.Insert(selectedIndex - 1, selectedItem);
                lstSelectedVideos.SelectedIndex = selectedIndex - 1;
            }
        }

        /// <summary>
        /// Event handler for the "Move Down" button.
        /// Moves the selected video down in the list.
        /// </summary>
        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            int selectedIndex = lstSelectedVideos.SelectedIndex;
            if (selectedIndex < lstSelectedVideos.Items.Count - 1 && selectedIndex != -1)
            {
                string selectedItem = lstSelectedVideos.SelectedItem.ToString();
                lstSelectedVideos.Items.RemoveAt(selectedIndex);
                lstSelectedVideos.Items.Insert(selectedIndex + 1, selectedItem);
                lstSelectedVideos.SelectedIndex = selectedIndex + 1;
            }
        }

        /// <summary>
        /// Event handler for MouseDown event to initiate drag-and-drop.
        /// </summary>
        private void lstSelectedVideos_MouseDown(object sender, MouseEventArgs e)
        {
            indexOfItemUnderMouseToDrag = lstSelectedVideos.IndexFromPoint(e.X, e.Y);
            if (indexOfItemUnderMouseToDrag != ListBox.NoMatches)
            {
                lstSelectedVideos.DoDragDrop(lstSelectedVideos.Items[indexOfItemUnderMouseToDrag], DragDropEffects.Move);
            }
        }

        /// <summary>
        /// Event handler for DragOver event to allow move effects.
        /// </summary>
        private void lstSelectedVideos_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        /// <summary>
        /// Event handler for DragDrop event to reorder items based on drop location.
        /// </summary>
        private void lstSelectedVideos_DragDrop(object sender, DragEventArgs e)
        {
            Point point = lstSelectedVideos.PointToClient(new Point(e.X, e.Y));
            int index = lstSelectedVideos.IndexFromPoint(point);
            if (index == ListBox.NoMatches)
            {
                index = lstSelectedVideos.Items.Count - 1;
            }

            string data = (string)e.Data.GetData(typeof(string));
            lstSelectedVideos.Items.Remove(data);
            lstSelectedVideos.Items.Insert(index, data);
        }
    }
}
