import dayjs from "dayjs";



export default class DateService {
  public static getTime(date: string): string {
    const dateDayJs = dayjs(date);

    const hour = dateDayJs.hour() >= 10 ? dateDayJs.hour() : `0${dateDayJs.hour()}`;
    const minutes = dateDayJs.minute() >= 10 ? dateDayJs.minute() : `0${dateDayJs.minute()}`;

    return `${hour}:${minutes}`;
  }

  public static getDateAndTime(date: string): string {
    const dateDayJs = dayjs(date);

    const hour = dateDayJs.hour() >= 10 ? dateDayJs.hour() : `0${dateDayJs.hour()}`;
    const minutes = dateDayJs.minute() >= 10 ? dateDayJs.minute() : `0${dateDayJs.minute()}`;
    const month = dateDayJs.month() >= 10 ? dateDayJs.month() : `0${dateDayJs.month()}`;

    return `${hour}:${minutes} ${dateDayJs.date()}/${month}/${dateDayJs.year()}`;
  }
}