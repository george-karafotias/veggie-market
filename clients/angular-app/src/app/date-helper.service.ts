import { Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root'
})
export class DateHelperService {

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

    calculateFullYearsList(start?: Date, end?: Date): number[] {
        const fullYearsList: number[] = [];
        if (!start || !end) return fullYearsList;

        const startYear = start.getFullYear();
        const endYear = end.getFullYear();
        const allYears = this.getYears(startYear, endYear);
        const years = new Set<number>();

        allYears.forEach(year => {
            if (year === startYear) {
                const firstDayOfYear = new Date(year, 0, 1); // January is index 0
                if (+start === +firstDayOfYear) {
                    years.add(year);
                }
            }

            if (year === endYear) {
                const lastDayOfYear = new Date(year, 11, 31); // December is index 11
                if (+end === +lastDayOfYear) {
                    years.add(year);
                }
            }

            if (year !== startYear && year !== endYear) {
                years.add(year);
            }
        });

        return Array.from(years);
    }

    private getYears(startYear: number, endYear: number): number[] {
        const years: number[] = [];
        for (let year = startYear; year <= endYear; year++) {
            years.push(year);
        }
        return years;
    }
}