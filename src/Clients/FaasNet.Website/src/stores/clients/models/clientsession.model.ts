import { TopicResult } from "./topic.model";

export class ClientSessionResult {
  pid: number = 0;
  purpose: number = 0;
  bufferCloudEvents: number = 0;
  state: number = 0;
  type: number = 0;
  creationDateTime: Date | undefined;
  topics: TopicResult[] = [];
}
