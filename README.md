# Gotify2Pushover

Push notifications from Gotify to Pushover
Register it as a client in Gotify and provide the token via an environment variable
Register it as an app in Pushover and provide its Userkey and AppKeys as environment variables
Written in C#/.NET.  Supports ws:// and wss://

```
## Docker Compose
  gotify2pushover:
    image: papirov/gotify2pushover:latest
    environment:
      - GOTIFY_TOKEN=${GOTIFY_TOKEN}
      - GOTIFY_HOST=ws://gotify     # or GOTIFY_HOST=wss://gotify.mydomain.com
      - PUSHOVER_USERKEY=${PUSHOVER_USERKEY}
      - PUSHOVER_APPKEY=${PUSHOVER_APPKEY}
```
