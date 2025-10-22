#pragma once
#include <string>
#include <vector>

class IAnalysedFile
{
public:
	virtual ~IAnalysedFile() = default;

	virtual std::string GetName() const = 0;
	virtual const std::vector<std::string>& GetDependencies() const = 0;
};

