using System;
class Program {
    public static void Main(){
        int n = Convert.ToInt32(Console.ReadLine());
        int m = Convert.ToInt32(Console.ReadLine());
        int cnt=0;
        for(int i=n; i<=m; i++){
            if(digitSum(i*i) == digitSum(i)*digitSum(i)){
                cnt++;
            }
        }
        Console.WriteLine(cnt);
    }
    public static int digitSum(int n){
        int sum =0;
        while(n>0){
            sum += (n%10);
            n /= 10;
        }
        return sum;
    }
}