export class SearchResult<T>
{
  constructor() {
    this.content = [];
  }

  startIndex: number | undefined;
  totalLength: number | undefined;
  count: number | undefined;
  content: T[];
}
