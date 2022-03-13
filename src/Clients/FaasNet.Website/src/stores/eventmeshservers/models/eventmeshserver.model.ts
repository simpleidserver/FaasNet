export class EventMeshServerResult {
  urn: string = "";;
  port: number = 0;
  countryIsoCode: string = "";
  latitude: number = 0;
  longitude: number = 0;
  createDateTime: Date | undefined;
  bridges: EventMeshServerBridgeResult[] = [];
}

export class EventMeshServerBridgeResult {
  urn: string = "";
  port: number = 0;
}
