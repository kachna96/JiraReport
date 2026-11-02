export function print() {
    window.print();
}

if (typeof window !== 'undefined') {
    window.printReport = function () {
        window.print();
    };
}
