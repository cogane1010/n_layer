using App.BookingOnline.Service.DTO.Common;
using Microsoft.Extensions.Logging;
using Renci.SshNet.Sftp;
using Renci.SshNet;
using System.Collections.Generic;
using System;
using System.IO;
using App.BookingOnline.Service.IService.Common;
using File = System.IO.File;
using Microsoft.Extensions.Configuration;

public class SftpService : ISftpService
{
    private readonly ILogger<SftpService> _logger;
    private readonly SftpConfig _config;
    private readonly string _doiXoatFolder = Path.Combine(Directory.GetCurrentDirectory(), "Doi_xoat\\");
    public IConfiguration Configuration { get; }
    public SftpService(ILogger<SftpService> logger, IConfiguration configuration)
    {
        _logger = logger;
        Configuration = configuration;
        _config = new SftpConfig
        {
            Host = Configuration.GetSection("ftpconfig").GetValue<string>("host"),
            Port = Configuration.GetSection("ftpconfig").GetValue<int>("port"),
            UserName = Configuration.GetSection("ftpconfig").GetValue<string>("userName"),
            PrivateKey = new PrivateKeyFile(Path.Combine(_doiXoatFolder, "key\\33333"))
        };
    }


    public IEnumerable<SftpFile> ListAllFiles(string remoteDirectory = ".")
    {
        using var client = new SftpClient(_config.Host, _config.Port == 0 ? 22 : _config.Port,
                        _config.UserName, new[] { _config.PrivateKey });
        try
        {
            client.Connect();
            return client.ListDirectory(remoteDirectory);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Failed in listing files under [{remoteDirectory}]");
            return null;
        }
        finally
        {
            client.Disconnect();
        }
    }

    public void UploadFile(string localFilePath, string remoteFilePath)
    {
        using var client = new SftpClient(_config.Host, _config.Port == 0 ? 22 : _config.Port,
                        _config.UserName, new[] { _config.PrivateKey });
        try
        {
            client.Connect();
            using var s = File.OpenRead(localFilePath);
            client.UploadFile(s, remoteFilePath);
            _logger.LogInformation($"Finished uploading file [{localFilePath}] to [{remoteFilePath}]");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Failed in uploading file [{localFilePath}] to [{remoteFilePath}]");
        }
        finally
        {
            client.Disconnect();
        }
    }

    public void DownloadFile(string remoteFilePath, string localFilePath)
    {
        using var client = new SftpClient(_config.Host, _config.Port == 0 ? 22 : _config.Port,
                        _config.UserName, new[] { _config.PrivateKey });
        try
        {
            client.Connect();
            using var s = File.Create(localFilePath);
            client.DownloadFile(remoteFilePath, s);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Failed in downloading file [{localFilePath}] from [{remoteFilePath}]");
        }
        finally
        {
            client.Disconnect();
        }
    }

    public string UploadFileTransc(string localFilePath, string filename, string userId, DateTime dateTrans)
    {
        using var client = new SftpClient(_config.Host, _config.Port == 0 ? 22 : _config.Port,
                        _config.UserName, new[] { _config.PrivateKey });
        try
        {
            client.Connect();
            var path = string.Format("{0}\\{1}", Directory.GetCurrentDirectory(), localFilePath);
            using var s = File.OpenRead(path);
            client.ChangeDirectory(@"/brg");
            client.UploadFile(s, filename);
            return string.Empty;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Failed in uploading file [{localFilePath}] to folder brg [{filename}]");
            return exception.Message;
        }
        finally
        {
            client.Disconnect();
        }
    }

    public void DeleteFile(string remoteFilePath)
    {
        using var client = new SftpClient(_config.Host, _config.Port == 0 ? 22 : _config.Port,
                        _config.UserName, new[] { _config.PrivateKey });
        try
        {
            client.Connect();
            client.DeleteFile(remoteFilePath);
            _logger.LogInformation($"File [{remoteFilePath}] deleted.");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Failed in deleting file [{remoteFilePath}]");
        }
        finally
        {
            client.Disconnect();
        }
    }


}