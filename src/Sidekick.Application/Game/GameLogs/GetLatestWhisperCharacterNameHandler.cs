using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sidekick.Domain.Game.GameLogs.Queries;
using Sidekick.Domain.Process;

namespace Sidekick.Application.Game.GameLogs
{
    public class GetLatestWhisperCharacterNameHandler : IQueryHandler<GetLatestWhisperCharacterNameQuery, string>
    {
        private readonly INativeProcess nativeProcess;

        public GetLatestWhisperCharacterNameHandler(
            INativeProcess nativeProcess)
        {
            this.nativeProcess = nativeProcess;
        }

        public Task<string> Handle(GetLatestWhisperCharacterNameQuery request, CancellationToken cancellationToken)
        {
            var clientLogFile = nativeProcess?.ClientLogPath;
            if (clientLogFile == null || !nativeProcess.IsPathOfExileInFocus)
            {
                return Task.FromResult<string>(null);
            }

            using var stream = new FileStream(clientLogFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            if (stream.Length == 0)
            {
                return Task.FromResult<string>(null);
            }

            stream.Position = stream.Length - 1;

            while (stream.Position > 0)
            {
                var currentLine = GetLine(stream);

                // See if the current line contains a received whisper
                var match = Regex.Match(currentLine, @"(@From){1}\s.+?(?=:)", RegexOptions.Singleline);
                if (match.Success)
                {
                    // No extract only character name
                    return Task.FromResult(match.Value[(match.Value.LastIndexOf(" ") + 1)..]);
                }

                stream.Position -= 1;
            }

            return Task.FromResult<string>(null);
        }


        /// <summary>
        /// Gets the line.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The current line of the stream position</returns>
        private string GetLine(Stream stream)
        {
            // while we have not yet reached start of file, read bytes backwards until '\n' byte is hit
            var lineLength = 0;
            while (stream.Position > 0)
            {
                stream.Position--;
                var byteFromFile = stream.ReadByte();

                if (byteFromFile < 0)
                {
                    // the only way this should happen is if someone truncates the file out from underneath us while we are reading backwards
                    throw new IOException("Error reading from file");
                }
                else if (byteFromFile == '\n')
                {
                    // we found the new line, break out, fs.Position is one after the '\n' char
                    break;
                }

                lineLength++;
                stream.Position--;
            }

            var oldPosition = stream.Position;
            // fs.Position will be right after the '\n' char or position 0 if no '\n' char
            var bytes = new BinaryReader(stream).ReadBytes(lineLength - 1);

            // -1 is the \n
            stream.Position = oldPosition - 1;
            return Encoding.UTF8.GetString(bytes).Replace(System.Environment.NewLine, string.Empty);
        }
    }
}
