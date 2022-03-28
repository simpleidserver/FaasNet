import { ClientSessionResult } from "./clientsession.model";

export class ClientResult {
  id: string = "";
  vpn: string = "";
  clientId: string = "";
  createDateTime: Date | null = null;
  purposes: number[] = [];
  sessions: ClientSessionResult[] = [];
}
