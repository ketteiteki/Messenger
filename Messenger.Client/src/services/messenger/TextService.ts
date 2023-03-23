export default class TextService {
  public static trimTextWithThirdDot(text: string, characterNumber: number) {
    if (text.length < characterNumber) return text;

    return `${text.slice(0, characterNumber).trim()}...`;
  }
}
