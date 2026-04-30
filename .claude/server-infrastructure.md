# Puck Server Infrastructure

## Host
- **Provider**: Linode/Akamai (~$30/month)
- **IP**: 172.233.221.208
- **OS**: Linux (Ubuntu)
- **Resources**: 2 CPU cores, ~4GB RAM
- **Uptime**: persistent (259+ days as of Apr 2026)
- **Access**: SSH via MCP tool (mcp__ssh-mcp__exec / mcp__ssh-mcp__sudo-exec)

## Server Instances

All servers run as the `puckd` system user under a systemd template service `puck@.service`.

| Instance | Directory | Game Port | Ping Port | CPU Affinity | Password | Notes |
|----------|-----------|-----------|-----------|--------------|----------|-------|
| server1 | /srv/puck/server1 | 7777 | 7778 | Core 0 | none | Primary |
| server2 | /srv/puck/server2 | 7779 | 7780 | Core 1 | none | Primary |
| server3 | /srv/puck/server3 | 7781 | 7782 | Both cores | 231 | Private/on-demand — keep stopped when not in use to save CPU |

## Service Management

```bash
# Start/stop/restart specific servers
systemctl start puck@server1 puck@server2
systemctl stop puck@server3
systemctl restart puck@server1

# Check status
systemctl status puck@server1 puck@server2 puck@server3 --no-pager

# View logs
journalctl -u puck@server1 --no-pager -n 50
```

All three services are `enabled` (auto-start on boot). Consider disabling server3 if it shouldn't start automatically:
```bash
systemctl disable puck@server3
```

## Systemd Setup

- **Template**: `/etc/systemd/system/puck@.service` — uses `%i` substitution for instance name
- **Overrides**: `/etc/systemd/system/puck@<instance>.service.d/override.conf` — sets CPUAffinity per instance
- **User**: `puckd` (dedicated system user)

## Server Config

Each server has a `config.json` in its directory. Key settings:
- Ports, name, password, maxPlayers, tick rates (all at 360Hz)
- Mods list (Workshop IDs)
- Admin Steam IDs
- Phase durations (warmup, playing, etc.)

## Firewall (ufw)

Ports 7777-7782 UDP are open. If adding more servers, remember to open both the game port and ping port.

## Resource Usage (3 servers running)

- Each Puck process uses ~350MB RAM and ~15-25% CPU even idle (360Hz tick loop)
- With all 3 running: 0% CPU idle, ~1GB RAM for game processes
- 2 servers is the comfortable limit for this box; server3 should only run when needed

## Game Binary

- Downloaded via SteamCMD (anonymous login, App ID 3481440)
- Binary: `Puck.x86_64`
- Launcher: `start_server.sh`
- Mods go in `<server>/Plugins/<ModName>/`

## Master Server Registration

- Servers register via WebSocket to `wss://puck1.nasejevs.com`
- Authentication response includes `isAuthenticated` flag (shows `False` but servers still appear in browser — this seems normal)

## Setup Guide Reference

Original setup guide: https://codionca.github.io/PuckGuide/
