import dayjs from "dayjs";

export default class DateService {
  public static getTime(date: string): string {
    const dateDayJs = dayjs(date);

    return `${
      dateDayJs.hour() >= 10 ? dateDayJs.hour() : `0${dateDayJs.hour()}`
    }:${
      dateDayJs.minute() >= 10 ? dateDayJs.minute() : `0${dateDayJs.minute()}`
    }`;
  }

  public static getDateAndTime(date: string): string {
    const dateDayJs = dayjs(date);

    return `${
      dateDayJs.hour() >= 10 ? dateDayJs.hour() : `0${dateDayJs.hour()}`
    }:${
      dateDayJs.minute() >= 10 ? dateDayJs.minute() : `0${dateDayJs.minute()}`
    } ${dateDayJs.date()}/${dateDayJs.month()}/${dateDayJs.year()}`;
  }
}
