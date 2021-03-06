using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sidekick.Domain.Clipboard;
using Sidekick.Domain.Game.Items.Commands;
using Sidekick.Domain.Game.Shortcuts.Commands;
using Sidekick.Domain.Keybinds;

namespace Sidekick.Application.Game.Shortcuts
{
    public class FindItemHandler : ICommandHandler<FindItemCommand, bool>
    {
        private readonly IKeybindsProvider keybindsProvider;
        private readonly IClipboardProvider clipboardProvider;
        private readonly IMediator mediator;

        public FindItemHandler(
            IKeybindsProvider keybindsProvider,
            IClipboardProvider clipboardProvider,
            IMediator mediator)
        {
            this.keybindsProvider = keybindsProvider;
            this.clipboardProvider = clipboardProvider;
            this.mediator = mediator;
        }

        public async Task<bool> Handle(FindItemCommand request, CancellationToken cancellationToken)
        {
            var text = await clipboardProvider.Copy();
            var item = await mediator.Send(new ParseItemCommand(text));

            if (item != null)
            {
                var clipboardContents = await clipboardProvider.GetText();

                await clipboardProvider.SetText(item.Name);

                keybindsProvider.PressKey("Ctrl+F");
                keybindsProvider.PressKey("Ctrl+A");
                keybindsProvider.PressKey("Paste");
                keybindsProvider.PressKey("Enter");

                await Task.Delay(100);
                await clipboardProvider.SetText(clipboardContents);

                return true;
            }

            return false;
        }
    }
}
