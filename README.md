# aicmd

Describe a task in plain English, get a ready-to-run bash command.

Press **Ctrl+G** at your shell prompt, describe what you want to do, and the generated command lands directly in your readline buffer — ready to review, edit, and run with Enter.

## Installation

### Snap

```bash
sudo snap install aicmd
```

### From source

Requires [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
dotnet publish src/Aicmd.Console/Aicmd.Console.csproj \
  --configuration Release \
  --runtime linux-x64 \
  --self-contained true \
  --output ./out
```

## Configuration

aicmd reads configuration from `appsettings.json` and from environment variables prefixed with `AICMD_`. Environment variables take precedence.

| Key | Description |
|-----|-------------|
| `ENDPOINT` | Base URL of an OpenAI-compatible API (e.g. `https://api.openai.com/v1`) |
| `MODEL` | Model name (e.g. `gpt-4o`) |
| `API_KEY` | API key for the endpoint |

**Via environment variables:**

```bash
export AICMD_ENDPOINT=https://api.openai.com/v1
export AICMD_MODEL=gpt-4o
export AICMD_API_KEY=sk-...
```

**Via snap configuration:**

```bash
sudo snap set aicmd endpoint=https://api.openai.com/v1
sudo snap set aicmd model=gpt-4o
sudo snap set aicmd api-key=sk-...
```

Works with any OpenAI-compatible provider: OpenAI, Azure OpenAI, Ollama, GitHub Models, etc.

## Shell integration

Source the script for your shell to enable the Ctrl+G key binding.

**bash** — add to `~/.bashrc`:

```bash
source /snap/aicmd/current/Shell/aicmd.bash
```

**zsh** — add to `~/.zshrc`:

```zsh
source /snap/aicmd/current/Shell/aicmd.zsh
```

If installed from source, source the scripts from your output directory instead.

To change the key binding, edit the script and replace `\C-g` (bash) or `^G` (zsh) with your preferred binding.

## Usage

1. Press **Ctrl+G** at the shell prompt.
2. Type a description of what you want to do and press Enter.
3. The generated command appears in your prompt buffer.
4. Review it, optionally edit it, then press Enter to run.
