#include "pch.h"
#include "IAnalysedFile.h"

class AnalysedFile : public IAnalysedFile
{
private:
	std::string name;
	std::vector<std::string> dependencies;

public:
	// Default constructor
	AnalysedFile(std::string name, std::vector<std::string> deps)
		: name(std::move(name)), dependencies(std::move(deps)) {}

	std::string GetName() const override
	{
		return name;
	}
	const std::vector<std::string>& GetDependencies() const override
	{
		return dependencies;
	}
};
