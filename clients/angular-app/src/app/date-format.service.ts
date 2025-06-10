import { Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root'
})
export class DateFormatService {

    formatDay(date: Date | undefined): string {
        if (date === undefined) return '';
        const year = date.getFullYear();
        let month = date.getMonth() + 1;
        let day = date.getDate();
        let dd = day < 10 ? '0' + day : day.toString();
        let mm = month < 10 ? '0' + month : month.toString();
        const formattedToday = dd + mm + year;
        return formattedToday;
    }

}