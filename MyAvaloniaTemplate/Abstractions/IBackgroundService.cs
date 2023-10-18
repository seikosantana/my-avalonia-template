using System;
using System.Threading.Tasks;

namespace MyAvaloniaTemplate.Abstractions;

public interface IBackgroundService : IDisposable
{
    public Task Cleanup();
}