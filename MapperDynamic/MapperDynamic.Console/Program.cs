using MapperDynamic.Models;
using MapperDynamic.Services;

Console.WriteLine("Hello, World!");

ObjectAssignementService<List<ClassA>, List<ClassB>> objectAssignmentService = new ObjectAssignementService<List<ClassA>, List<ClassB>>();
objectAssignmentService.Setup();
objectAssignmentService.Mapping();

//TODO: to be developed and check viability