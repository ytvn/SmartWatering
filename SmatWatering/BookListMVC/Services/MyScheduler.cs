using SmartWatering.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartWatering.Services
{
    public static class MyScheduler
    {
        public static void Reset()
        {
            SchedulerService.Instance.ScheduleTask(1, 1, 1, false, ()=> { });
        }
        public static void Interval(int hour, int min, IntervalType type, double interval, bool status, Action task)
        {
            switch (type)
            {
                case IntervalType.IntervalInSeconds:
                    interval = interval / 3600;
                    break;
                case IntervalType.IntervalInMinutes:
                    interval = interval / 60;
                    break;
                case IntervalType.IntervalInHours:
                    break;
                case IntervalType.IntervalInDays:
                    interval = interval * 24;
                    break;
            }
            SchedulerService.Instance.ScheduleTask(hour, min, interval, status, task);
        }
        public static void IntervalInSeconds(int hour, int sec, double interval, bool status, Action task)
        {
            interval = interval / 3600;
            SchedulerService.Instance.ScheduleTask(hour, sec, interval,status, task);
        }
        public static void IntervalInMinutes(int hour, int min, double interval, bool status, Action task)
        {
            interval = interval / 60;
            SchedulerService.Instance.ScheduleTask(hour, min, interval, status, task);
        }
        public static void IntervalInHours(int hour, int min, double interval, bool status, Action task)
        {
            SchedulerService.Instance.ScheduleTask(hour, min, interval, status, task);
        }
        public static void IntervalInDays(int hour, int min, double interval, bool status, Action task)
        {
            interval = interval * 24;
            SchedulerService.Instance.ScheduleTask(hour, min, interval, status, task);
        }
    }
}
