using System;

namespace Authifi.Contracts.Services
{
    public interface IApplicationInfoService
    {
        Version GetVersion();
    }
}
