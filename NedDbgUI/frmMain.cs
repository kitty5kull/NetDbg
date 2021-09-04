using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Windows.Forms;
using ProxyLib;

namespace NetDbgUI
{
	public partial class frmMain : Form
	{
		private Piper _piper;
		private ConcurrentQueue<string> LogLines = new ConcurrentQueue<string>();
		private ConcurrentQueue<string> ErrorLines = new ConcurrentQueue<string>();
		private ConcurrentQueue<string> InterpreterLines = new ConcurrentQueue<string>();
		public const int MAX_LOG_LINES = 10000;
		private IInterpreter _interpreter;

		public frmMain()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			_piper?.Dispose();
			base.Dispose(disposing);
		}

		private void frmMain_Load(object sender, EventArgs e)
		{
			_interpreter = new EchoInterpreter();
			_interpreter.CommandPending += _interpreter_CommandPending;
			_interpreter.OutputLinePending += _interpreter_OutputLinePending;

			_piper = new Piper();
			_piper.OutputLineReceived += PiperOutputLineReceived;
			_piper.ErrorLineReceived += Piper_ErrorLineReceived;
			_piper.CommandExecuted += Piper_CommandExecuted;

			_interpreter.Init();
			Left = 2500;
			WindowState = FormWindowState.Maximized;
		}

		private void _interpreter_OutputLinePending(object sender, string e)
		{
			InterpreterLines.Enqueue(e);
			while (InterpreterLines.Count > MAX_LOG_LINES)
				InterpreterLines.TryDequeue(out _);
		}

		private void _interpreter_CommandPending(object sender, string e)
		{
			_piper.SendCommand(e);
		}

		private void Piper_ErrorLineReceived(object sender, string e)
		{
			ErrorLines.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:m:ss.fff ") + e);
			while (ErrorLines.Count > MAX_LOG_LINES)
				ErrorLines.TryDequeue(out _);
			_interpreter.InterpretErrorLine(e);
		}

		private void Piper_CommandExecuted(object sender, string e)
		{
			AddLogLine("> " + e);
		}

		private void PiperOutputLineReceived(object sender, string e)
		{
			AddLogLine(e);
			_interpreter.InterpretOutputLine(e);
		}

		private void AddLogLine(string line)
		{
			if (!chkCapturePackets.Checked
			    && (line.StartsWith("packet") || line.StartsWith("> packet")))
				return;

			LogLines.Enqueue(DateTime.Now.ToString("HH:mm:ss.fff ") + line);
			while (LogLines.Count > MAX_LOG_LINES)
				LogLines.TryDequeue(out _);
		}

		private void timRefresh_Tick(object sender, EventArgs e)
		{
			string text = string.Join(Environment.NewLine, LogLines);
			if (text != txtDebug.Text)
			{
				txtDebug.Text = text;
				txtDebug.SelectionStart = txtDebug.Text.Length;
				txtDebug.ScrollToCaret();
			}

			text = string.Join(Environment.NewLine, ErrorLines);
			if (text != txtError.Text)
			{
				txtError.Text = text;
				txtError.SelectionStart = txtError.Text.Length;
				txtError.ScrollToCaret();
			}

			//text = string.Join(Environment.NewLine, InterpreterLines.Reverse());
			text = string.Join(Environment.NewLine, InterpreterLines) + Environment.NewLine;
			if (text != txtInterpreter.Text)
			{
				txtInterpreter.Text = text;
				txtInterpreter.SelectionStart = txtInterpreter.Text.Length;
				txtInterpreter.ScrollToCaret();
			}

			if (lblStatus.Text != _interpreter.CurrentStatus)
			{
				lblStatus.Text = _interpreter.CurrentStatus;
				lblStatus.Visible =!string.IsNullOrWhiteSpace(_interpreter.CurrentStatus);
			}
		}

		private void txtCommand_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == 13)
			{
				_piper.SendCommand(txtCommand.Text);
				txtCommand.Text = "";
				e.Handled = true;
			}
		}

		private void txtInterpreterCommand_KeyPress(object sender, KeyPressEventArgs e)
		{
		if (e.KeyChar == 13)
			{
				_interpreter.SendCommand(txtInterpreterCommand.Text);
				//txtInterpreterCommand.Text = "";
				e.Handled = true;
			}
		}
	}
}
