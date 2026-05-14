Check the status of the Puck game servers using the SSH MCP tools.

If the user specified servers as an argument: $ARGUMENTS
Otherwise, check both primary servers (server1 and server2).

Run the following commands using `mcp__ssh-mcp__exec` (or `mcp__ssh-mcp__sudo-exec` for systemctl):

1. **Service status** — check if the servers are running:
```
systemctl status puck@server1 puck@server2 --no-pager -n 0
```

2. **Recent logs** — grab the last 20 log lines from each server to check for errors or activity:
```
journalctl -u puck@server1 --no-pager -n 20
journalctl -u puck@server2 --no-pager -n 20
```

3. **Resource usage** — check CPU and memory for the Puck processes:
```
ps aux --no-headers -C Puck.x86_64
```

4. **Uptime and load** — quick system health check:
```
uptime
```

If the user asked about a specific server (e.g. "1" or "server1"), only check that one. If they said "all", also include server3 (ports 7781/7782, the private/on-demand server).

Present the results as a clear summary for each server:
- **Status**: Running / Stopped / Failed
- **Uptime**: How long the service has been running
- **CPU / Memory**: From the ps output
- **Recent activity**: Notable lines from the logs (errors, player connections, warnings). Don't dump raw logs — summarize what's happening.

If any server is down or showing errors, flag it clearly.
