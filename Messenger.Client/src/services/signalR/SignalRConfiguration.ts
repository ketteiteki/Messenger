import * as signalR from "@microsoft/signalr";


class SignalRConfiguration {
  public connection: signalR.HubConnection | null = null;

  public buildConnection = (accessToken: string) => {
    const connectionBuilder = new signalR.HubConnectionBuilder();
    this.connection = connectionBuilder
      .withUrl(process.env.REACT_APP_BASE_API + "/notification", {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
        accessTokenFactory: () => accessToken,
      })
      .build();
  };
}

export const signalRConfiguration = new SignalRConfiguration();