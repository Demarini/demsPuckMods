Ask the user which Puck game servers they want to restart. The available servers are:

- **server1** — ports 7777/7778
- **server2** — ports 7779/7780
- **server3** — ports 7781/7782 (password protected)

They can specify individual servers (e.g. "1", "2", "3"), multiple (e.g. "1 and 3"), or "all".

If the user provided their choice as an argument: $ARGUMENTS

Once you know which servers to restart, run the restart command using the `mcp__ssh-mcp__sudo-exec` tool:

```
systemctl restart puck@server1 puck@server2 puck@server3
```

Only include the servers they requested. After restarting, check the status with:

```
systemctl status puck@server1 puck@server2 puck@server3 --no-pager -n 3
```

Report back whether each restarted server is active and running.
