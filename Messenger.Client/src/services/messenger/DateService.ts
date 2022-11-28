import dayjs from 'dayjs';


export default class DateService {
    
    public static getTime(date: string): string {

        const dateDayJs = dayjs(date); 

        return `${dateDayJs.hour()}:${dateDayJs.minute()}`;
    }

    public static getDateAndTime(date: string): string {

        const dateDayJs = dayjs(date); 

        return `${dateDayJs.hour()}:${dateDayJs.minute()} ${dateDayJs.date()}/${dateDayJs.month()}/${dateDayJs.year()}`;
    }
} 