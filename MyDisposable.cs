using Microsoft.Extensions.Logging;

namespace DotNetConsoleApp;

internal class MyDisposable(ILogger<MyDisposable> logger)
	: IDisposable
{
	private readonly ILogger _logger = logger;

	#region Disposable
	private bool _disposed;

	void IDisposable.Dispose()
	{
		_logger.LogInformation("In IDisposable.Dispose()");
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected void Dispose(bool disposing)
	{
		_logger.LogInformation("In Dispose({disposing})", disposing);
		if (!_disposed)
		{
			if (disposing)
			{
				DisposeManagedResources();
			}

			DisposeUnmanagedResources();
			_disposed = true;
		}
	}

	protected virtual void DisposeManagedResources()
	{
		_logger.LogInformation("In DisposeManagedResources()");
	}
	protected virtual void DisposeUnmanagedResources()
	{
		_logger.LogInformation("In DisposeUnmanagedResources()");
	}

	~MyDisposable()
	{
		_logger.LogInformation("In ~MyDisposable()");
		Dispose(false);
	}
	#endregion
}
