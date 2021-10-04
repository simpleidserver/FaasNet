import { Injectable, Pipe, PipeTransform } from "@angular/core";
import { TranslateService } from "@ngx-translate/core";

@Injectable()
@Pipe({
  name: 'translateobj',
  pure: false
})
export class TranslateObjPipe implements PipeTransform {
  constructor(private ts: TranslateService) { }

  transform(translations: any) {
    if (!translations || translations.length === 0) {
      return null;
    }

    const lng = this.ts.currentLang;
    const filteredTranslations = translations.filter((t : any) => {
      return t.Language === lng;
    });
    if (filteredTranslations.length !== 1) {
      return "unknown";
    }

    return filteredTranslations[0].Description;
  }
}
