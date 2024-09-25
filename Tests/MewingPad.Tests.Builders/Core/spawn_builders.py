import os

members = []

cwd = "/home/daria/Документы/bmstu-testing/Common/MewingPad.Common.Entities"
for filename in os.listdir(cwd):
    if not filename.endswith('.cs'):
        continue
    with open(os.path.join(cwd, filename), 'r') as f:
        for l in f:
            if l.count("{ get; set; }") != 0:
                members.append(l.strip())
            if l.count("public class") != 0:
                members.append(l.strip())
        members.append([])

f = None
for m in members:
    if len(m) == 0:
        print()
        print(f'    public {classname} Build()', file=f)
        print('    {', file=f)
        print(f'        return _{classname.lower()};', file=f)
        print('    }', file=f)
        print('}', file=f)
        print(file=f)
        f.close()
    elif m.startswith('public class'):
        classname = m.split()[-1]
        buildername = f'{classname}CoreModelBuilder'
        f = open(f'/home/daria/Документы/bmstu-testing/Tests/MewingPad.Tests.DataAccess.UnitTests/Builders/Core/{buildername}.cs', 'w')
        print('namespace MewingPad.Tests.DataAccess.UnitTests.Builders;\n', file=f)
        print('using MewingPad.Common.Entities;\n', file=f)
        print(f'public class {buildername}', file=f)
        print('{', file=f)
        print(f'    private {classname} _{classname.lower()} = new();', file=f)
    else:
        membername = m.split()[2]
        argname = membername[0].lower() + membername[1:]
        membertype = m.split()[1]
        print(file=f)
        print(f'    public {buildername} With{membername}({membertype} {argname})', file=f)
        print('    {', file=f)
        print(f'        _{classname.lower()}.{membername} = {argname};', file=f)
        print('        return this;', file=f)
        print('    }', file=f)
