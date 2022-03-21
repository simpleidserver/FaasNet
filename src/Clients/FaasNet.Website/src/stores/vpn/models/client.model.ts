import { ClientSessionResult } from "./clientsession.model";

export class ClientResult {
  clientId: string = "";
  createDateTime: Date | null = null;
  purposes: number[] = [];
  sessions: ClientSessionResult[] = [];
}
